import { useState } from "react"
import React from 'react'
import AddItemModal from "../../Modal/AddItem/AddItemModal"
import "./Footer.css"
import { ToastContainer, toast } from "react-toastify"
import API_BASE_URL from "../../config"

const Footer = () => {

    const [showAddModal, setShowAddModal] = useState(false);

    const openAddModal = () => {
        setShowAddModal(true);
    }

    const closeAddModal = () => {
        setShowAddModal(false)
    };
    const handleAddItem = async (newItem) => {
        try {
            const response = await fetch(`${API_BASE_URL}/notifications/todo/addNotification`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(newItem)
            });
            if (response.ok) {
                toast.success('Message sent successfully!');
                closeAddModal();
            } else {
                toast.error('Failed to send message.');
            }
        } catch (error) {
            console.error('Error sending message:', error);
            toast.error('An error occurred while sending the message.');
        }
    };

    return (
        <footer className="footer">
            <div className="footer-content">
                <h4>If you have any problem, please send a message to Admin:</h4>
                <button onClick={openAddModal}>Send Message</button>
                <AddItemModal
                    isOpenAdd={showAddModal}
                    onCancelAdd={closeAddModal}
                    onConfirmAdd={handleAddItem}

                />
            </div>
        </footer>
    )
}

export default Footer