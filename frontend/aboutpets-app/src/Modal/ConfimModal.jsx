import React from 'react';
import './ConfirmModal.css'; // Make sure to style your modal appropriately

const ConfirmModal = ({ isOpen, onCancel, onConfirm, title, message, confirmButtonText, confirmButtonClass }) => {
    return (
        <div className={`modal ${isOpen ? 'open' : ''}`}>
            <div className='modal-content'>
                <h2>{title}</h2>
                <p>{message}</p>
                <div className='modal-buttons'>
                    <button onClick={onCancel}>Cancel</button>
                    <button onClick={onConfirm} className={confirmButtonClass}>{confirmButtonText}</button>
                </div>
            </div>
        </div>
    );
}

export default ConfirmModal;