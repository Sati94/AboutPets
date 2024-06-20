import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import API_BASE_URL from '../../../config';
import './AdminUpdateOrder.css';
import { AuthContext } from '../../../AuthContext/AuthContext';
import { ToastContainer, toast } from 'react-toastify';


const AdminUpdateOrder = () => {
    const { authState } = useContext(AuthContext);
    const { orderId } = useParams();
    const navigate = useNavigate();
    const [order, setOrder] = useState(null);
    const [orderItems, setOrderItems] = useState([]);
    const [status, setStatus] = useState(0);



    useEffect(() => {
        const fetchOrderDetails = async () => {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/order/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                const data = await response.json();
                setOrder(data);
            } catch (error) {
                toast.error("Failed to fetch order details.");
            }
        };

        if (authState.token) {
            fetchOrderDetails();
        }
    }, [authState.token, authState.role, orderId]);

    useEffect(() => {
        const fetchOrderItems = async () => {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/order/orderItems/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                const data = await response.json();
                setOrderItems(data);
            } catch (error) {
                toast.error("Failed to fetch order items.");
            }
        };

        if (authState.token) {
            fetchOrderItems();
        }
    }, [authState.token, authState.role, orderId]);
    const handleStatusChange = async (event) => {
        const newStatus = event.target.value;
        setStatus(newStatus);
        console.log(newStatus);
        console.log(status);
    }

    const handleSubmit = async (e) => {
        e.preventDefault();



        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/order/update/${orderId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(status)
            });

            if (response.ok) {
                toast.success("Order status updated!");
                navigate('/admin/orders')
            } else {
                toast.error("Failed to update order status.");
            }
        } catch (error) {
            toast.error("Failed to update order status.");
        }

    };
    const handleDeleteItem = async (orderItemId) => {
        try {
            const { token, role, userId } = authState;
            const response = await fetch(`${API_BASE_URL}/orderitem/remove?orderId=${orderId}&orderItemId=${orderItemId}&userId=${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                }
            });

            if (response.ok) {
                setOrderItems(orderItems.filter(item => item.orderItemId !== orderItemId));
                toast.success("Item deleted!");
                console.log(orderItems)
            } else {
                toast.error("Failed to delete item.");
            }
        } catch (error) {
            toast.error("Failed to delete item.");
        }
    };
    return (
        <div>
            <h1>Order Details</h1>
            {order && (
                <div>
                    <ul>
                        <li><strong>Id:</strong> {order.orderId}</li>
                        <li><strong>Date:</strong> {order.orderDate}</li>
                        <li><strong>Total Price:</strong>{order.totalPrice}</li>
                        <li>
                            <strong>Status:</strong>
                            <select value={status} onChange={handleStatusChange}>
                                <option value="1">Pending</option>
                                <option value="2">Processing</option>
                                <option value="3">Shipped</option>
                                <option value="4">Delivered</option>
                                <option value="5">Cancelled</option>
                            </select>
                        </li>
                        <li><strong>User Id:</strong>{order.userId}</li>
                    </ul>
                </div>
            )}
            <h2>Order Items</h2>
            {orderItems.length > 0 ? (
                orderItems.map(item => (
                    <div key={item.orderItemId} className='order-item'>
                        <ul>
                            <li><strong>Product Id:</strong> {item.productId}</li>
                            <li><strong>Quantity:</strong> {item.quantity}</li>
                            <li><strong>Price:</strong> {item.price}</li>
                        </ul>
                        <button onClick={() => handleDeleteItem(item.orderItemId)}>Delete</button>
                    </div>
                ))
            ) : (
                <p>No items found for this order</p>
            )}
            <form onSubmit={handleSubmit}>
                <button type="submit">Update Status</button>
            </form>
            <ToastContainer />
        </div>
    );
}

export default AdminUpdateOrder