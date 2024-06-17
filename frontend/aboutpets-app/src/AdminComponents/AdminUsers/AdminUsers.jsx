import React, { useContext, useState, useEffect } from 'react'
import { AuthContext } from '../../AuthContext/AuthContext'
import { useNavigate } from 'react-router-dom';
import API_BASE_URL from '../../config';
import SearchInput from '../../Components/SearchInput/SearchInput';
import { ToastContainer, toast } from 'react-toastify';
import "./AdminUsers.css";





const AdminUsers = () => {

    const { authState } = useContext(AuthContext);
    const [users, setUsers] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredUsers, setFilteredUsers] = useState([]);
    const [userProfileId, setUserProfileId] = useState("");
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
        setUserProfileId(userId);
        navigate(`/admin/users/${userId}`)
    };


    /*useEffect(() => {
        async function fetchUserProfile() {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/UserProfile/user/profile/${userProfileId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                if (response.ok) {
                    const data = await response.json();
                    console.log("User profile data:", data);

                } else {
                    console.error("Error fetching user profile:", response.status, response.statusText);
                }
            } catch (error) {
                console.error("Error fetching user profile:", error);
            }
        }

        if (userProfileId !== "") {
            fetchUserProfile();
        }
    }, [userProfileId, authState]);*/


    const handleDelete = async (userId) => {

        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/user/delete/${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                }
            })
            if (response.ok) {
                toast.success("Element deleted!");
            }
            else {
                toast.error("Somehing is worng!");
            }

        }
        catch (error) {
            console.log("Error:", error);
            toast.error("Somehing is worng!")
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
                            <button className='delete' onClick={() => handleDelete(user.id)}>Delete</button>

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
        </div>
    )

}
export default AdminUsers