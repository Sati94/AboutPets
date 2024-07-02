import React, { useState, useEffect, useContext } from 'react'
import API_BASE_URL from '../../../config'
import { useParams, useNavigate } from 'react-router-dom'
import { AuthContext } from '../../../AuthContext/AuthContext'
import ConfirmModal from '../../../Modal/ConfimModal'



const UpdateUserProfile = () => {
    const { userId } = useParams();
    const { authState } = useContext(AuthContext);
    const navigate = useNavigate();
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [userProfile, setUserProfile] = useState({
        firstName: "",
        lastName: "",
        phoneNumber: "",
        address: "",
        bonus: ""
    });
    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setUserProfile({ ...userProfile, [name]: value });
    };
    useEffect(() => {
        const fetchProduct = async () => {
            const { token, role } = authState;
            try {
                const response = await fetch(`${API_BASE_URL}/UserProfile/user/profile/${userId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                const data = await response.json();
                setUserProfile(data);
            } catch (error) {
                console.error("Error fetching product:", error);
            }
        };

        if (authState.token) {
            fetchProduct();
        }
    }, [authState, userId]);
    const handleSubmit = (e) => {
        e.preventDefault();
        setShowUpdateModal(true);
    };
    const handleConfirmUpdate = async (e) => {

        const { token, role } = authState;

        try {
            const response = await fetch(`${API_BASE_URL}/admin/update/profile/${userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(userProfile)
            });

            if (response.ok) {
                const data = await response.json();
                console.log("UserProfile updated:", data);
                navigate('/admin/users');
            } else {
                console.error("Error updating profile:", response.status, response.statusText);
            }

        } catch (error) {
            console.error("Error updating profile:", error);
        }
        setShowUpdateModal(false);
    };
    return (
        <div>
            <h1>Update Profile</h1>
            <form onSubmit={handleSubmit}>
                <label> First Name:</label>
                <input
                    type="text"
                    name="firstName"
                    value={userProfile.firstName}
                    onChange={handleInputChange}
                    required
                />
                <label>Last Name:</label>
                <input
                    type="text"
                    name="lastName"
                    value={userProfile.lastName}
                    onChange={handleInputChange}
                    required
                />
                <label>Phone Number:</label>
                <input
                    type="text"
                    name="phoneNumber"
                    value={userProfile.phoneNumber}
                    onChange={handleInputChange}
                    required
                />
                <label>Address</label>
                <input
                    type="text"
                    name="address"
                    value={userProfile.address}
                    onChange={handleInputChange}
                    required
                />
                <label>Bonus:</label>
                <input
                    type="text"
                    name="bonus"
                    value={userProfile.bonus}
                    onChange={handleInputChange}
                    required
                />

                <button type="submit">Update Profile</button>
            </form>
            <ConfirmModal
                isOpen={showUpdateModal}
                onCancel={() => setShowUpdateModal(false)}
                onConfirm={handleConfirmUpdate}
                title="Confirm Update"
                message="Are you sure you want to update this profile?"
                confirmButtonText="Update"
                confirmButtonClass="update"
            />

        </div>
    );

}

export default UpdateUserProfile