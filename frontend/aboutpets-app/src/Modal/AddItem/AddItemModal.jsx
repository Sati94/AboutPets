import React, { useState } from 'react'
import './AddItemModal.css'

const AddItemModal = ({ isOpenAdd, onCancelAdd, onConfirmAdd, currentUser }) => {

    const [sender, setSender] = useState(currentUser.userName || '');
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');

    const handleConfirmAdd = () => {
        onConfirmAdd({ sender, title, description });
        setSender(currentUser.userName || '');
        setTitle('');
        setDescription('');
    }


    if (!isOpenAdd) return null;
    return (
        <div className="add-modal">
            <div className="add-modal-content">
                <h2>Add New Item</h2>
                <div className="add-modal-body">
                    <label>
                        Sender:
                        <input type="text" value={sender} onChange={(e) => setSender(e.target.value)} />
                    </label>
                    <label>
                        Title:
                        <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} />
                    </label>
                    <label>
                        Description:
                        <textarea value={description} onChange={(e) => setDescription(e.target.value)} />
                    </label>
                </div>
                <div className="add-modal-actions">
                    <button className='cancel-modal-button' onClick={onCancelAdd}>Cancel</button>
                    <button className='add-modal-button' onClick={handleConfirmAdd}>Add</button>
                </div>
            </div>
        </div>
    )
}

export default AddItemModal