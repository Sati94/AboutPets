import React from 'react'
import "./DeleteConfirmModal.css"

const DeleteConfirmModal = ({ isOpen, onCancel, onConfirm }) => {
    return (
        <div className={`modal ${isOpen ? 'open' : ''}`}>
            <div className='modal-content'>
                <h2>Confirm Delete</h2>
                <p>Are you sure want to delete this item ?</p>
                <div className='modal-buttons'>
                    <button onClick={onCancel}>Cancel</button>
                    <button onClick={onConfirm} className='delete'>Delete</button>
                </div>
            </div>
        </div>
    )
}

export default DeleteConfirmModal