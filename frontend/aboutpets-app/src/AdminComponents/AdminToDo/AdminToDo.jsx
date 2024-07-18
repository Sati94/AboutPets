import React, { useEffect, useState, useContext, useCallback } from 'react';
import './AdminToDo.css';
import { ToastContainer, toast } from 'react-toastify';
import API_BASE_URL from '../../config';
import { AuthContext } from '../../AuthContext/AuthContext';
import ConfirmModal from '../../Modal/ConfimModal';
import AddItemModal from '../../Modal/AddItem/AddItemModal';
import SearchInput from '../../Components/SearchInput/SearchInput';

const AdminToDo = () => {
    const [todoElements, setTodoElements] = useState([]);
    const { authState } = useContext(AuthContext);
    const [filteredTodo, setFilteredTodo] = useState([]);
    const [showAddModal, setShowAddModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [itemToDelete, setItemToDelete] = useState(null);
    const [draggingElement, setDraggingElement] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [needsFetch, setNeedsFetch] = useState(true);


    const statusMap = {
        new: 1,
        processing: 2,
        completed: 3
    };

    const formatDate = (createdDate) => {
        const date = new Date(createdDate);
        const year = date.getFullYear();
        const month = ('0' + (date.getMonth() + 1)).slice(-2);
        const day = ('0' + date.getDate()).slice(-2);
        const hours = ('0' + date.getHours()).slice(-2);
        const minutes = ('0' + date.getMinutes()).slice(-2);

        return `${year}-${month}-${day} ${hours}:${minutes}`;
    };
    const fetchTodo = useCallback(async () => {
        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/notifications/all`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                }
            });
            const data = await response.json();
            setTodoElements(data);
            setFilteredTodo(data);
        } catch (error) {
            console.error('Error fetching todos');
        }
    }, [authState.token, authState.role]);

    useEffect(() => {
        if (needsFetch) {
            fetchTodo();
            setNeedsFetch(false);
        }
    }, [authState.token, authState.role, needsFetch]);

    useEffect(() => {
        const data = todoElements.filter(element => {
            const searchLower = searchTerm.toLowerCase();
            return (
                element.title.toLowerCase().includes(searchLower)
            );
        });
        setFilteredTodo(data);
    }, [searchTerm, todoElements]);

    const openDeleteModal = (element) => {
        setItemToDelete(element);
        setShowDeleteModal(true);
    };

    const cancelDelete = () => {
        setShowDeleteModal(false);
    };
    const handleSearch = (term) => {
        setSearchTerm(term);
    };

    const confirmToDelete = async () => {
        if (itemToDelete) {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/Notification/notifications/deleteTodoItem/${itemToDelete.id}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                if (response.ok) {
                    toast.success('Element deleted!');
                    const updatedElement = todoElements.filter(element => element.id !== itemToDelete.id);
                    setTodoElements(updatedElement);
                    setFilteredTodo(updatedElement);
                    setShowDeleteModal(false);
                } else {
                    toast.error('Something is wrong!');
                }
            } catch (error) {
                console.log('Error:', error);
                toast.error('Something is wrong!');
            }
        }
    };

    const handleDragStart = (event, element) => {
        setDraggingElement(element);
        event.dataTransfer.effectAllowed = 'move';
    };

    const handleDragOver = event => {
        event.preventDefault();
        event.dataTransfer.effectAllowed = 'move';
    };

    const handleDrop = async (event, status) => {
        event.preventDefault();
        if (draggingElement) {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/updateTodoItemStatus/${draggingElement.id}?status=${statusMap[status]}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                if (response.ok) {
                    const updatedElement = await response.json();
                    const updatedElements = todoElements.map(element =>
                        element.id === updatedElement.id ? updatedElement : element
                    );
                    setTodoElements(updatedElements);
                    setFilteredTodo(updatedElements);
                    toast.success('Status updated!');
                    setNeedsFetch(true);

                } else {
                    toast.error('Failed to update status!');
                }
            } catch (error) {
                console.log('Error:', error);
                toast.error('Something went wrong!');
            }
        }
        setDraggingElement(null);
    };

    const getStatusColumn = (status, title, elements) => (
        <div className={`status-column ${status}`} onDragOver={handleDragOver} onDrop={event => handleDrop(event, status)}>
            <h2>{title}</h2>
            {elements.filter(element => element.status === statusMap[status]).map(element => (
                <div
                    key={element.id}
                    className='element-item'
                    draggable
                    onDragStart={event => handleDragStart(event, element)}
                    onClick={() => openDeleteModal(element)}
                >
                    <ul>
                        <li><strong>Sender:</strong> {element.sender}</li>
                        <li><strong>Title:</strong> {element.title}</li>
                        <li><strong>Description:</strong> {element.description}</li>
                        <li><strong>Date:</strong> {formatDate(element.createdDate)}</li>
                    </ul>
                    <div className='button-actions-item'>
                        <button className='delete-item' onClick={() => openDeleteModal(element)}>Delete</button>
                    </div>
                </div>
            ))}
        </div>
    );

    const openAddModal = () => {
        setShowAddModal(true);
    };

    const closeAddModal = () => {
        setShowAddModal(false);
    };

    const handleAddItem = async (newItem) => {
        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/notifications/todo/addNotification`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(newItem)
            });
            if (response.ok) {
                const addedItem = await response.json();
                setTodoElements([...todoElements, addedItem]);
                setFilteredTodo([...filteredTodo, addedItem]);
                toast.success('New item added!');
                setShowAddModal(false);
            } else {
                toast.error('Failed to add new item!');
            }
        } catch (error) {
            console.log('Error:', error);
            toast.error('Something went wrong!');
        }
    };

    return (
        <div className='todoElements-container'>
            <div className="search-bar text-center">
                <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Todo Title..." />
            </div>
            <div className='status-container'>
                {getStatusColumn('new', 'New', filteredTodo)}
                {getStatusColumn('processing', 'Processing', filteredTodo)}
                {getStatusColumn('completed', 'Completed', filteredTodo)}
            </div>
            <div className="add-button-container">
                <button className="add-notification-button" onClick={openAddModal}>Add new Notification</button>
            </div>
            <ToastContainer />
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDelete}
                onConfirm={confirmToDelete}
                title="Confirm Delete"
                message="Are you sure you want to delete this item?"
                confirmButtonText="Delete"
                confirmButtonClass="delete"
            />
            <AddItemModal
                isOpenAdd={showAddModal}
                onCancelAdd={closeAddModal}
                onConfirmAdd={handleAddItem}
                currentUser={'Admin'}
            />
        </div>
    );
};

export default AdminToDo;