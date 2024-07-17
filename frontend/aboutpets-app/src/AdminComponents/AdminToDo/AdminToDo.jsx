import React, { useEffect, useState, useContext } from 'react'
import './AdminToDo.css'
import { ToastContainer, toast } from 'react-toastify'
import API_BASE_URL from '../../config'
import { AuthContext } from '../../AuthContext/AuthContext'
import SearchInput from '../../Components/SearchInput/SearchInput';
import ConfirmModal from '../../Modal/ConfimModal';



const AdminToDo = () => {

    const [todoElements, setTodoElements] = useState([]);
    const { authState } = useContext(AuthContext);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredTodo, setFilteredTodo] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [itemToDelete, setItemToDelete] = useState(null);

    useEffect(() => {

        async function fetchTodo() {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/notifications/all`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }

                })
                const data = await response.json();
                console.log(data);
                setTodoElements(data);

            } catch (error) {
                console.error("Error fetching todos");
            }


        }
        if (authState.token) {
            fetchTodo();

        }
    }, []);

    useEffect(() => {
        const data = todoElements.filter(elements => {
            const searchLower = searchTerm.toLowerCase();

            return (
                elements.title.toLowerCase().includes(searchLower)
            )

        });
        setFilteredTodo(data);

    }, [searchTerm, todoElements]);

    const handleSearch = (term) => {
        setSearchTerm(term);
    }

    const openDeleteModal = (element) => {
        setItemToDelete(element);
        setShowDeleteModal(true);
    };


    const cancelDelete = () => {
        setShowDeleteModal(false);
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
                })
                if (response.ok) {
                    toast.success("Element deleted!");
                    const updatedelement = todoElements.filter((element) => element.id !== itemToDelete.id);
                    setTodoElements(updatedelement);
                    setFilteredTodo(updatedelement);
                    setShowDeleteModal(false);
                }
                else {
                    toast.error("Somehing is worng!");
                }

            }
            catch (error) {
                console.log("Error:", error);
                toast.error("Somehing is worng!")
            }
        }
    };


    return (
        <div className='todoElements-container'>
            <h1>ToDo</h1>
            <div className="search-bar text-center">
                <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Todo Title..." />
            </div>
            <div className='element-list'>
                {filteredTodo.map(element => (

                    <div key={element.Id} className='element-item'>
                        <ul>
                            <li><strong>Sender:</strong> {element.sender}</li>
                            <li><strong>Title:</strong> {element.title}</li>
                            <li><strong>Description:</strong> {element.description}</li>
                            <li><strong>Date:</strong> {element.createdDate}</li>
                        </ul>
                        <div className='button-actions'>
                            <button className='delete' onClick={() => openDeleteModal(element)}>Delete</button>
                        </div>

                    </div>



                ))}
            </div>
            <ToastContainer />
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDelete}
                onConfirm={confirmToDelete}
                title="Confirm Delete"
                message="Are you sure want to delete this item?"
                confirmButtonText="Delete"
                confirmButtonClass="delete"
            />

        </div >
    )
}

export default AdminToDo