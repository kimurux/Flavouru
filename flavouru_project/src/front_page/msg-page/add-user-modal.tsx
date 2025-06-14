"use client"

import { useState, useEffect } from "react"
import axios from "axios"

interface User {
  id: string
  username: string
  email?: string
  avatar?: string
}

interface AddUserModalProps {
  isOpen: boolean
  onClose: () => void
  onUserAdded: (user: User) => void
  currentUserId: string
}

export function AddUserModal({ isOpen, onClose, onUserAdded, currentUserId }: AddUserModalProps) {
  const [searchQuery, setSearchQuery] = useState("")
  const [searchResults, setSearchResults] = useState<User[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [page, setPage] = useState(1)
  const [hasMore, setHasMore] = useState(true)

  // Search for users using the available endpoints
  const searchUsers = async (query: string, pageNum = 1) => {
    if (!query.trim()) {
      setSearchResults([])
      return
    }

    setIsLoading(true)
    try {
      let users: User[] = []

      // Try searching by username first
      try {
        const usernameResponse = await axios.get(`http://92.248.255.123:5000/users/username/${query}`)
        if (usernameResponse.data && usernameResponse.data.id !== currentUserId) {
          users.push({
            id: usernameResponse.data.id,
            username: usernameResponse.data.username,
            email: usernameResponse.data.email,
            avatar: usernameResponse.data.avatar,
          })
        }
      } catch (error) {
        // Username not found, try email
        try {
          const emailResponse = await axios.get(`http://92.248.255.123:5000/users/email/${query}`)
          if (emailResponse.data && emailResponse.data.id !== currentUserId) {
            users.push({
              id: emailResponse.data.id,
              username: emailResponse.data.username,
              email: emailResponse.data.email,
              avatar: emailResponse.data.avatar,
            })
          }
        } catch (emailError) {
          // Neither username nor email found
        }
      }

      // If no exact match, get all users and filter (with pagination)
      if (users.length === 0) {
        try {
          const allUsersResponse = await axios.get("http://92.248.255.123:5000/users", {
            params: {
              page: pageNum,
              limit: 20, // Limit results to prevent performance issues
            },
          })

          const filteredUsers =
            allUsersResponse.data?.filter(
              (user: any) =>
                user.id !== currentUserId &&
                (user.username.toLowerCase().includes(query.toLowerCase()) ||
                  user.email?.toLowerCase().includes(query.toLowerCase())),
            ) || []

          users = filteredUsers.map((user: any) => ({
            id: user.id,
            username: user.username,
            email: user.email,
            avatar: user.avatar,
          }))

          // Check if there are more results
          setHasMore(filteredUsers.length === 20)
        } catch (error) {
          console.error("Error fetching all users:", error)
        }
      }

      if (pageNum === 1) {
        setSearchResults(users)
      } else {
        setSearchResults((prev) => [...prev, ...users])
      }
    } catch (error) {
      console.error("Error searching users:", error)
      setSearchResults([])
    } finally {
      setIsLoading(false)
    }
  }

  // Debounced search
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      setPage(1)
      searchUsers(searchQuery, 1)
    }, 300)

    return () => clearTimeout(timeoutId)
  }, [searchQuery])

  const loadMore = () => {
    if (!isLoading && hasMore) {
      const nextPage = page + 1
      setPage(nextPage)
      searchUsers(searchQuery, nextPage)
    }
  }

  const handleStartChat = async (user: User) => {
    try {
      // Send an initial message to create the chat
      const response = await axios.post("http://92.248.255.123:5000/chat/messages", {
        content: "👋 Hello! Let's start chatting.",
        sender_id: currentUserId,
        receiver_id: user.id,
        message_type: "text",
      })

      if (response.data) {
        // Add user to chat list
        onUserAdded(user)

        // Create notification for the other user
        await axios.post("http://92.248.255.123:5000/notifications", {
          type: "chat_message",
          title: "New Message",
          message: "You have a new message",
          user_id: user.id,
          sender_id: currentUserId,
          chat_message_id: response.data.id,
        })

        onClose()
        setSearchQuery("")
        setSearchResults([])
      }
    } catch (error) {
      console.error("Error starting chat:", error)
    }
  }

  const resetModal = () => {
    setSearchQuery("")
    setSearchResults([])
    setPage(1)
    setHasMore(true)
  }

  const handleClose = () => {
    resetModal()
    onClose()
  }

  if (!isOpen) return null

  return (
    <div className="modal-overlay" onClick={handleClose}>
      <div className="modal-content add-user-modal" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Start New Chat</h3>
          <button className="modal-close" onClick={handleClose}>
            ×
          </button>
        </div>

        <div className="modal-body">
          <div className="search-container">
            <input
              type="text"
              placeholder="Search users by username or email..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="search-input"
              autoFocus
            />
          </div>

          <div className="search-results">
            {isLoading && searchResults.length === 0 && (
              <div className="loading-state">
                <div className="spinner"></div>
                <span>Searching users...</span>
              </div>
            )}

            {!isLoading && searchQuery && searchResults.length === 0 && (
              <div className="empty-state">
                <p>No users found matching "{searchQuery}"</p>
              </div>
            )}

            {searchResults.length > 0 && (
              <div className="user-list">
                {searchResults.map((user) => (
                  <div key={user.id} className="user-item">
                    <div className="user-avatar">
                      {user.avatar ? (
                        <img src={user.avatar || "/placeholder.svg"} alt={user.username} />
                      ) : (
                        <div className="avatar-placeholder">{user.username.charAt(0).toUpperCase()}</div>
                      )}
                    </div>
                    <div className="user-info">
                      <div className="user-name">{user.username}</div>
                      {user.email && <div className="user-email">{user.email}</div>}
                    </div>
                    <button className="start-chat-btn" onClick={() => handleStartChat(user)}>
                      Start Chat
                    </button>
                  </div>
                ))}

                {hasMore && !isLoading && (
                  <button className="load-more-btn" onClick={loadMore}>
                    Load More
                  </button>
                )}

                {isLoading && searchResults.length > 0 && (
                  <div className="loading-more">
                    <div className="spinner"></div>
                    <span>Loading more...</span>
                  </div>
                )}
              </div>
            )}

            {!searchQuery && (
              <div className="empty-state">
                <p>Type a username or email to search for users</p>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
