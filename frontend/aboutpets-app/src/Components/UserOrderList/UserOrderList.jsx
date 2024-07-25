import React, { useEffect, useState, useContext } from 'react'
import { AuthContext } from '../../AuthContext/AuthContext'
import API_BASE_URL from '../../config'
import "./UserOrderList.css"
import { useNavigate } from 'react-router-dom'
import { ToastContainer, toast } from 'react-toastify'

const UserOrderList = () => {

    const navigate = useNavigate();
    const { authState } = useContext(AuthContext);
    const [orderList, setOrderList] = useState([]);
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

        fetchUserOrders(userId, token, role);
    }, []);


    const fetchUserOrders = async (userId, token, role) => {
        try {
            const response = await fetch(`${API_BASE_URL}/order/user/${userId}`, {
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
            setOrderList(data);
            console.log(data)
            setLoading(false);
        } catch (error) {
            setError(error);
            setLoading(false);
        }
    };


    if (loading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>Error: {error.message}</div>;
    }

    return (
        <div className='order-container'>
            <h1>My Orders</h1>
            {orderList.length === 0 ? (
                <p>No orders found.</p>
            ) : (
                <div className='order-list'>
                    {orderList.map(order => (
                        <div key={order.orderId} className='order-item-data'>
                            <p><strong>Order ID:</strong> {order.orderId}</p>
                            <p><strong>Order Date:</strong> {new Date(order.orderDate).toLocaleDateString()}</p>
                            <p><strong>Total Price:</strong> ${order.totalPrice}</p>
                            <p><strong>Delivery Type:</strong> {order.deliveryType}</p>
                            <p><strong>Order Status:</strong> {order.orderStatuses}</p>
                            <p><strong>Address:</strong> {order.streetAddress}, {order.city}, {order.country}</p>
                            <p><strong>Order Items:</strong> {order.orderItems.length > 0 ? order.orderItems.join(', ') : 'No items in this order'}</p>
                        </div>
                    ))}
                </div>
            )}
            <ToastContainer />
        </div>
    );
}

export default UserOrderList