import React, { useContext } from 'react';
import './UserProfile.css';
import { useState, useEffect } from 'react';
import API_BASE_URL from '../../config';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/ReactToastify.css'
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../AuthContext/AuthContext';
import ConfirmModal from '../../Modal/ConfimModal';


const UserProfile = () => {

    const navigate = useNavigate();
    const { authState } = useContext(AuthContext);

    const [profile, setProfile] = useState({
        userProfileId: "",
        firstName: "",
        lastName: "",
        phoneNumber: "",
        address: "",
        bonus: 0
    });

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showUpdateConfirm, setShowUpdateConfirm] = useState(false);


    useEffect(() => {
        const { token, role, userId } = authState;

        if (!userId) {
            toast.error('First, you have to Log In!');
            navigate('/login');
            return;
        }

        fetchUserProfile(userId, token, role);
    }, []);


    const fetchUserProfile = async (userId, token, role) => {
        try {
            const response = await fetch(`${API_BASE_URL}/UserProfile/user/profile/${userId}`, {
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                    "Role": role
                }
            });

            if (!response.ok) {
                throw new Error("Network response is bad");
            }

            const data = await response.json();
            setProfile(data);
            setLoading(false);
        } catch (error) {
            setError(error);
            setLoading(false);
        }
    };

    const handleChange = (e) => {
        const { name, value } = e.target;
        setProfile(prevProfile => ({
            ...prevProfile,
            [name]: value
        }))
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        setShowUpdateConfirm(true);
    }
    const oncancel = () => {
        setShowUpdateConfirm(false);
    }
    const submit = async () => {
        const { token, role, userId } = authState;

        fetch(`${API_BASE_URL}/update/user/profile/${userId}`, {
            method: 'PUT',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
                "Role": role
            },
            body: JSON.stringify(profile)
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response is bad');
                }
                return response.json();
            })
            .then(() => {
                toast.success('Profile updated successfully!')
                navigate("/");
            })
            .catch(error => {
                setError(error);
                toast.error('Error updating profile')
            });
        setShowUpdateConfirm(false);
    };

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error loading profile: {error.message}</p>;

    return (

        <div className="profile-container">
            <h2>{authState.userName} Profile</h2>
            <form className="profile-form" onSubmit={handleSubmit}>
                <div>
                    <label>First Name:</label>
                    <input type="text" name="firstName" value={profile.firstName} onChange={handleChange} />
                </div>
                <div>
                    <label>Last Name:</label>
                    <input type="text" name="lastName" value={profile.lastName} onChange={handleChange} />
                </div>
                <div>
                    <label>Phone Number:</label>
                    <input type="text" name="phoneNumber" value={profile.phoneNumber} onChange={handleChange} />
                </div>
                <div>
                    <label>Address:</label>
                    <input type="text" name="address" value={profile.address} onChange={handleChange} />
                </div>
                <div>
                    <label>Bonus:</label>
                    <input type="text" name="bonus" value={profile.bonus} disabled />
                </div>
                <button type="submit"> Update Profile</button>
            </form>

            <ToastContainer />
            <ConfirmModal
                isOpen={showUpdateConfirm}
                onCancel={oncancel}
                onConfirm={submit}
                title="Confirm Update"
                message="Are you sure you want to update your Profile?"
                confirmButtonText="Update"
                confirmButtonClass="update"
            />
        </div>
    )
}

export default UserProfile