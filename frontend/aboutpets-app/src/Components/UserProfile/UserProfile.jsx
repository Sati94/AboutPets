import React from 'react';
import './UserProfile.css';
import { useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import API_BASE_URL from '../../config';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/ReactToastify.css'
import { useNavigate } from 'react-router-dom';


const UserProfile = () => {

    const navigate = useNavigate();
    const userId = Cookies.get("userId");
    const userName = Cookies.get("userUserName");
    const userToken = Cookies.get("userToken");
    const userRole = Cookies.get("userRole")

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


    useEffect(() => {
        if (!userId) {
            toast.error('First, you have to Log In!');

            navigate('/login');

            return;
        }

        fetch(`${API_BASE_URL}/UserProfile/user/profile/${userId}`, {
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${userToken}`,
                "Role": { userRole }
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error("Network response is bad");
                }
                return response.json();
            })
            .then(data => {
                setProfile(data);
                setLoading(false);
            })
            .catch(error => {
                setError(error);
                setLoading(false);
            });
    }, [userId, userToken, navigate, userRole]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setProfile(prevProfile => ({
            ...prevProfile,
            [name]: value
        }))
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        fetch(`${API_BASE_URL}/update/user/profile/${userId}`, {
            method: 'PUT',
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${userToken}`,
                "Role": { userRole }
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
            })
            .catch(error => {
                setError(error);
                toast.error('Error updating profile')
            });
    };

    if (loading) return <p>Loading...</p>;
    if (error) return <p>Error loading profile: {error.message}</p>;

    return (

        <div className="profile-container">
            <h2>{userName} Profile</h2>
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
        </div>
    )
}

export default UserProfile