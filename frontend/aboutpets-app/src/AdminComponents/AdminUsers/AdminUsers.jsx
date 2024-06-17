import React, { useContext, useState, useEffect } from 'react'
import { AuthContext } from '../../AuthContext/AuthContext'
import { useNavigate } from 'react-router-dom';
import API_BASE_URL from '../../config';
import SearchInput from '../../Components/SearchInput/SearchInput';
import { ToastContainer, toast } from 'react-toastify';
import DeleteConfirmModal from '../../Modal/DeleteConfirmModal';
import "./AdminUsers.css";





const AdminUsers = () => {

    const { authState } = useContext(AuthContext);
    const [users, setUsers] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [userId, setUserId] = useState("");
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDelete, setUserToDelete] = useState(null);
    const navigate = useNavigate();


    useEffect(() => {

        async function fetchUsers() {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/allUser`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }

                })
                const data = await response.json();
                setUsers(data);

            } catch (error) {
                console.error("Error fetching useres");
            }


        }
        if (authState.token) {
            fetchUsers();

        }
    }, [users])

    useEffect(() => {
        const data = users.filter(user => {
            const searchLower = searchTerm.toLowerCase();

            return (
                user.userName.toLowerCase().includes(searchLower)
            )

        });
        setFilteredUsers(data);

    }, [searchTerm, users]);

    const handleSearch = (term) => {
        setSearchTerm(term);

    }
    const handleProfileClick = (userId) => {
        setUserId(userId);
        navigate(`/admin/users/${userId}`)
    };


    const toggleDeleteModal = () => {
        setShowDeleteModal(!showDeleteModal);
    }
    const openDeleteModal = (user) => {
        setUserToDelete(user);
        setShowDeleteModal(true);
    }


    const cancelDelete = () => {
        setShowDeleteModal(false);
    };

    const confirmToDelete = async () => {
        if (userToDelete) {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/user/delete/${userToDelete.id}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                })
                if (response.ok) {
                    toast.success("Element deleted!");
                    const updatedUsers = users.filter((user) => user.id !== userToDelete.id);
                    setUsers(updatedUsers);
                    setFilteredUsers(updatedUsers);
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
        <div>
            <h1>Users</h1>
            <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Users..." />
            <div className='user-list'>
                {filteredUsers.map(user => (
                    user.userName !== 'admin' ? (
                        <div key={user.Id} className='user-item'>
                            <ul>
                                <li><strong>Name:</strong> {user.userName}</li>
                                <li><strong>Email:</strong> {user.email}</li>
                            </ul>
                            <button className='profile' onClick={() => handleProfileClick(user.id)}>Profile</button>
                            <button className='delete' onClick={() => openDeleteModal(user)}>Delete</button>

                        </div>

                    ) : (
                        <div key={user.Id} className='user-item'>
                            <ul>
                                <li><strong>Name:</strong> {user.userName}</li>
                                <li><strong>Email:</strong> {user.email}</li>
                            </ul>
                        </div>)

                ))}
            </div>
            <ToastContainer />
            <DeleteConfirmModal isOpen={showDeleteModal} onCancel={cancelDelete} onConfirm={confirmToDelete} />

        </div >
    )

}
export default AdminUsers