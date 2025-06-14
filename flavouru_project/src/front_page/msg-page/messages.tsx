import '../fp.css';
import './messages.css';
import logofull from '../../assets/img/logofull.svg';
import { Link } from 'react-router-dom';
import { useState, useEffect, useRef } from 'react';
import axios from 'axios';
import { AddUserModal } from "./add-user-modal.tsx"

axios.defaults.baseURL = 'http://92.248.255.123:5000';
const token = localStorage.getItem('token');
const user1_id = localStorage.getItem('user_id')
if (token) axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;

console.log(user1_id)

type Message = {
  id: string
  content: string
  sender_id: string
  receiver_id: string
  is_read: boolean
  created_at: string
  message_type?: string
}

type ChatUser = {
  id: string
  username: string
  email?: string
  avatar?: string
  lastMessage?: string
  lastMessageTime?: string
  unreadCount?: number
}

// Local storage key for persisting chat users
const CHAT_USERS_STORAGE_KEY = `chat_users_${user1_id}`

function MsgPage() {
  const [chatUsers, setChatUsers] = useState<ChatUser[]>([])
  const [currentChatUserId, setCurrentChatUserId] = useState<string | null>(null)
  const [currentChatUser, setCurrentChatUser] = useState<ChatUser | null>(null)
  const [messages, setMessages] = useState<Message[]>([])
  const [inputValue, setInputValue] = useState<string>("")
  const [isModalOpen, setModalOpen] = useState<boolean>(false)
  const [isRefreshing, setIsRefreshing] = useState(false)
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const pollingIntervalRef = useRef<NodeJS.Timeout | null>(null)

  // Load chat users from localStorage on component mount
  useEffect(() => {
    const savedUsers = localStorage.getItem(CHAT_USERS_STORAGE_KEY)
    if (savedUsers) {
      try {
        const parsedUsers = JSON.parse(savedUsers)
        setChatUsers(parsedUsers)
      } catch (error) {
        console.error("Error parsing saved chat users:", error)
      }
    }
    // Also fetch from API
    fetchChatList()
  }, [])

  // Save chat users to localStorage whenever chatUsers changes
  useEffect(() => {
    if (chatUsers.length > 0) {
      localStorage.setItem(CHAT_USERS_STORAGE_KEY, JSON.stringify(chatUsers))
    }
  }, [chatUsers])

  // Load chat list from API
  const fetchChatList = async () => {
    try {
      const response = await axios.get(`/chat/list?user_id=${user1_id}`)
      const chatData = response.data || []

      // Transform API response to our ChatUser format
      const transformedChats = chatData.map((chat: any) => ({
        id: chat.user_id || chat.id,
        username: chat.username || chat.name || `User ${chat.id}`,
        email: chat.email,
        avatar: chat.avatar,
        lastMessage: chat.last_message,
        lastMessageTime: chat.last_message_time,
        unreadCount: 0,
      }))

      // Merge with existing users from localStorage
      setChatUsers((prevUsers) => {
        const existingIds = prevUsers.map((u) => u.id)
        const newUsers = transformedChats.filter((u: ChatUser) => !existingIds.includes(u.id))
        return [...prevUsers, ...newUsers]
      })
    } catch (error) {
      console.error("Error fetching chat list:", error)
    }
  }

  // Polling for new messages when a chat is selected
  useEffect(() => {
    if (currentChatUserId) {
      if (pollingIntervalRef.current) {
        clearInterval(pollingIntervalRef.current)
      }

      pollingIntervalRef.current = setInterval(() => {
        fetchMessages(currentChatUserId, false)
      }, 3000)

      return () => {
        if (pollingIntervalRef.current) {
          clearInterval(pollingIntervalRef.current)
        }
      }
    }
  }, [currentChatUserId])

  // Auto-scroll to bottom
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" })
  }, [messages])

  const fetchMessages = async (userId: string, showLoading = true) => {
    try {
      if (showLoading) setIsRefreshing(true)

      const response = await axios.get(`/chat/messages/${userId}`, {
        params: {
          current_user_id: user1_id,
          page: 1,
          page_size: 100,
        },
      })

      const newMessages = response.data || []
      setMessages(newMessages)

      // Mark unread messages as read
      const unreadMessages = newMessages.filter((msg: Message) => !msg.is_read && msg.sender_id === userId)

      if (unreadMessages.length > 0) {
        markMessagesAsRead(unreadMessages.map((msg: Message) => msg.id))
      }
    } catch (error) {
      console.error("Error loading messages:", error)
      setMessages([])
    } finally {
      if (showLoading) setIsRefreshing(false)
    }
  }

  const handleChatSelect = async (userId: string, user: ChatUser) => {
    setCurrentChatUserId(userId)
    setCurrentChatUser(user)
    await fetchMessages(userId, true)
  }

  const markMessagesAsRead = async (messageIds: string[]) => {
    try {
      await axios.post("/chat/messages/mark-read", {
        message_ids: messageIds,
        user_id: user1_id,
      })
      fetchChatList()
    } catch (error) {
      console.error("Error marking messages as read:", error)
    }
  }

  const handleSend = async () => {
    if (!inputValue.trim() || !currentChatUserId) return

    try {
      const response = await axios.post("/chat/messages", {
        content: inputValue,
        sender_id: user1_id,
        receiver_id: currentChatUserId,
        message_type: "text",
      })

      if (response.data) {
        // Create notification for receiver
        await axios.post("/notifications", {
          type: "chat_message",
          title: "New Message",
          message: `You have a new message`,
          user_id: currentChatUserId,
          sender_id: user1_id,
          chat_message_id: response.data.id,
        })

        // Update last message in chat list
        setChatUsers((prev) =>
          prev.map((user) =>
            user.id === currentChatUserId
              ? { ...user, lastMessage: inputValue, lastMessageTime: new Date().toISOString() }
              : user,
          ),
        )

        // Immediately fetch new messages
        await fetchMessages(currentChatUserId, false)
      }

      setInputValue("")
    } catch (error) {
      console.error("Error sending message:", error)
    }
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }

  const handleRefresh = () => {
    if (currentChatUserId) {
      fetchMessages(currentChatUserId, true)
    }
    fetchChatList()
  }

  const handleUserAdded = async (user: { id: string; username: string; email?: string }) => {
    // Add user to chat list immediately
    const newChatUser: ChatUser = {
      id: user.id,
      username: user.username,
      email: user.email,
      lastMessage: "👋 Hello! Let's start chatting.",
      lastMessageTime: new Date().toISOString(),
      unreadCount: 0,
    }

    // Update the chat users list
    setChatUsers((prev) => {
      const userExists = prev.some((chatUser) => chatUser.id === user.id)
      if (!userExists) {
        return [newChatUser, ...prev]
      }
      return prev
    })

    // Automatically select the new chat
    setCurrentChatUserId(user.id)
    setCurrentChatUser(newChatUser)

    // Fetch messages for this user (will show the initial message)
    setTimeout(() => {
      fetchMessages(user.id, false)
    }, 500)
  }

  const handleRemoveUser = (userId: string) => {
    setChatUsers((prev) => prev.filter((user) => user.id !== userId))

    // If the removed user was currently selected, clear the selection
    if (currentChatUserId === userId) {
      setCurrentChatUserId(null)
      setCurrentChatUser(null)
      setMessages([])
    }

    // Clear from localStorage
    const updatedUsers = chatUsers.filter((user) => user.id !== userId)
    localStorage.setItem(CHAT_USERS_STORAGE_KEY, JSON.stringify(updatedUsers))
  }

  const getUsernameById = (userId: string): string => {
    if (userId === user1_id) {
      return "You"
    }
    const user = chatUsers.find((u) => u.id === userId)
    return user?.username || "Unknown User"
  }

  return (
    <div className='main'>
      <div className='body__container'>
        <div className='nav__container'>
          <nav className='nav'>
            <div className='nav__logo'>
              <img src={logofull} alt="Logo" style={{ height: '100%' }} />
            </div>
            <div className='catalogue__container'>
              <div className='catalogue'>
                <Link to="/Main-page"><button>Главное</button></Link>
                <Link to="/Forum"><button>Форум</button></Link>
                <Link to="/Information"><button>Инфо</button></Link>
                <Link to="/Receipts"><button>Рецепты</button></Link>
                <Link to="/Messages"><button>Сообщения</button></Link>
                <Link to="/Profile"><button>Профиль</button></Link>
              </div>
            </div>
          </nav>
        </div>
        <div className='main-body__container'>
             <div className="chat-main__container">
                <div className="chat__info">
                    <div className="chat__lookup">
                    <button className="add-chat__button" onClick={() => setModalOpen(true)} aria-label="Start new chat">
                        +
                    </button>
                    </div>
                    <div className="chat-user__desc">
                    {currentChatUser ? (
                        <div className="current-chat-header">
                        <div className="current-chat-avatar">
                            {currentChatUser.avatar ? (
                            <img src={currentChatUser.avatar || "/placeholder.svg"} alt={currentChatUser.username} />
                            ) : (
                            <div className="avatar-placeholder">{currentChatUser.username.charAt(0).toUpperCase()}</div>
                            )}
                        </div>
                        <div className="current-chat-info">
                            <div className="current-chat-name">{currentChatUser.username}</div>
                            <div className="current-chat-status">
                            {currentChatUser.lastMessageTime
                                ? `Last seen: ${new Date(currentChatUser.lastMessageTime).toLocaleString()}`
                                : "Online"}
                            </div>
                        </div>
                        <button
                            className="refresh-button"
                            onClick={handleRefresh}
                            disabled={isRefreshing}
                            title="Refresh messages"
                        >
                            {isRefreshing ? "⟳" : "↻"}
                        </button>
                        </div>
                    ) : (
                        "Select a chat to start messaging"
                    )}
                    </div>
                </div>

                <div className="chat__container">
                    <div className="chat-users__container">
                    {chatUsers.length > 0 ? (
                        chatUsers.map((user) => (
                        <div
                            key={user.id}
                            className={`user__container ${user.id === currentChatUserId ? "active" : ""}`}
                            onClick={() => handleChatSelect(user.id, user)}
                        >
                            <div className="user-avatar">{user.username.charAt(0).toUpperCase()}</div>
                            <div className="user-info">
                            <div className="user-name">{user.username}</div>
                            {user.lastMessage && <div className="last-message">{user.lastMessage}</div>}
                            </div>
                            <div className="user-actions">
                            {user.unreadCount && user.unreadCount > 0 && <div className="unread-badge">{user.unreadCount}</div>}
                            <button
                                className="remove-user-btn"
                                onClick={(e) => {
                                e.stopPropagation()
                                handleRemoveUser(user.id)
                                }}
                                title="Remove user from chat list"
                            >
                                ×
                            </button>
                            </div>
                        </div>
                        ))
                    ) : (
                        <div className="no-chats">
                        <p>Нет чатов</p>
                        <p>Нажмите на + кнопку, чтобы добавить чат</p>
                        </div>
                    )}
                    </div>

                    <div className="chat-text__container">
                    {currentChatUserId ? (
                        <>
                        <div className="chat__messages">
                            {messages.map((msg) => {
                            const isMyMessage = msg.sender_id === user1_id
                            return (
                                <div key={msg.id} className={`message-wrapper ${isMyMessage ? "my-message" : "other-message"}`}>
                                <div className="message-bubble">
                                    <div className="message-header">
                                    <span className="message-sender">{getUsernameById(msg.sender_id)}</span>
                                    </div>
                                    <div className="message-content">{msg.content}</div>
                                    <div className="message-time">
                                    {new Date(msg.created_at).toLocaleTimeString([], {
                                        hour: "2-digit",
                                        minute: "2-digit",
                                    })}
                                    </div>
                                </div>
                                </div>
                            )
                            })}
                            <div ref={messagesEndRef} />
                        </div>

                        <div className="chat-input__container">
                            <input
                            type="text"
                            value={inputValue}
                            onChange={(e) => setInputValue(e.target.value)}
                            onKeyPress={handleKeyPress}
                            placeholder="Type a message..."
                            className="chat-input"
                            />
                            <button onClick={handleSend} className="send-button" disabled={!inputValue.trim()}>
                            Send
                            </button>
                        </div>
                        </>
                    ) : (
                        <div className="select-chat-prompt">
                        <div className="prompt-content">
                            <h3>Welcome to Messages</h3>
                            <p>Select an existing chat or start a new conversation</p>
                            <button className="start-chat-cta" onClick={() => setModalOpen(true)}>
                            Start New Chat
                            </button>
                        </div>
                        </div>
                    )}
                    </div>
                </div>

                <AddUserModal
                    isOpen={isModalOpen}
                    onClose={() => setModalOpen(false)}
                    onUserAdded={handleUserAdded}
                    currentUserId={user1_id || ""}
                />
                </div>
        </div>
    </div>
    </div>
  );
}

export default MsgPage;
