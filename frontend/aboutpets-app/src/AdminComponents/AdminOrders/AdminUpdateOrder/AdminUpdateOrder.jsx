import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import API_BASE_URL from '../../../config';
import './AdminUpdateOrder.css';
import { AuthContext } from '../../../AuthContext/AuthContext';
import { ToastContainer, toast } from 'react-toastify';
import ConfirmModal from '../../../Modal/ConfimModal';

const AdminUpdateOrder = () => {
    const { authState } = useContext(AuthContext);
    const { orderId } = useParams();
    const navigate = useNavigate();
    const [order, setOrder] = useState(null);
    const [orderItems, setOrderItems] = useState([]);
    const [deleteToOrderItem, setDeleteToOrderItem] = useState(null);
    const [status, setStatus] = useState(0);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [id, setId] = useState("");
    const [formData, setFormData] = useState({
        country: "",
        city: "",
        streetAddress: "",
        deliveryType: "0"
    })
    const handleSubmitForm = async (e) => {
        e.preventDefault();
        const { token, role } = authState;
        const requestBody = {
            country: formData.country,
            city: formData.city,
            streetAddress: formData.streetAddress,
            deliveryType: parseInt(formData.deliveryType)
        };
        try {
            const response = await fetch(`${API_BASE_URL}/update-order-delivery/${orderId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Failed to update order delivery type and address: ${errorMessage}`);
            }

            toast.success('Order delivery type and address updated successfully!');

        } catch (error) {
            toast.error('Error updating order delivery type and address!');
            console.log(error)
        }
    };

    const toggleDeleteModal = () => {
        setShowDeleteModal(!showDeleteModal);
    };

    const openDeleteModal = (orderItem) => {
        setDeleteToOrderItem(orderItem);
        setShowDeleteModal(true);
    };

    const openUpdateModal = () => {
        setShowUpdateModal(true);
    };

    const cancelDelete = () => {
        setShowDeleteModal(false);
    };

    const cancelUpdate = () => {
        setShowUpdateModal(false);
    };

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
                const user = data.userId;
                setId(user);
                setFormData({
                    country: data.country || "",
                    city: data.city || "",
                    streetAddress: data.streetAddress || "",
                    deliveryType: data.deliveryType || "0"
                });
            } catch (error) {
                toast.error("Failed to fetch order details.");
            }
        };

        if (authState.token) {
            fetchOrderDetails();
        }
    }, [authState.token, authState.role, orderId, orderItems]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value,
        });
    };

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

    }


    const handleConfirmUpdate = async () => {

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
    const handleDeleteItem = async () => {
        if (deleteToOrderItem) {
            try {
                const { token, role } = authState;

                console.log('Delete Parameters:', {
                    orderId,
                    orderItemId: deleteToOrderItem.orderItemId,
                    id
                });
                const response = await fetch(`${API_BASE_URL}/orderitem/remove?orderId=${orderId}&orderItemId=${deleteToOrderItem.orderItemId}&userId=${id}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });

                if (response.ok) {
                    const updatedOrderItems = orderItems.filter(item => item.orderItemId !== deleteToOrderItem.orderItemId)
                    setOrderItems(updatedOrderItems);
                    toast.success("Item deleted!");
                    setShowDeleteModal(false);

                    if (updatedOrderItems.length === 0) {
                        navigate('/admin/orders');
                    }
                } else {

                    toast.error("Failed to delete item.");
                }
            } catch (error) {
                toast.error("Failed to delete item.");
            }
        }

    };

    return (
        <div className='update-order-container'>
            <h1>Order Details</h1>
            {order && (
                <div className='order-details'>
                    <ul>
                        <li><strong>Id:</strong> {order.orderId}</li>
                        <li><strong>Date:</strong> {order.orderDate}</li>
                        <li><strong>Total Price:</strong>{order.totalPrice}</li>
                        <li>
                            <strong>Status:</strong>
                            <select value={status} onChange={handleStatusChange}>
                                <option value="0">Select Status </option>
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
            <div className="form-group">
                <label>Country:</label>
                <input type="text" name="country" value={formData.country} onChange={handleChange} />
            </div>
            <div className="form-group">
                <label>City:</label>
                <input type="text" name="city" value={formData.city} onChange={handleChange} />
            </div>
            <div className="form-group">
                <label>Street Address:</label>
                <input type="text" name="streetAddress" value={formData.streetAddress} onChange={handleChange} />
            </div>
            <div className="form-group">
                <label>Delivery Type:</label>
                <select name="deliveryType" value={formData.deliveryType} onChange={handleChange}>
                    <option value="0">Select Delivery Type</option>
                    <option value="1">GLS</option>
                    <option value="2">Posta</option>
                    <option value="3">DPD</option>

                </select>
            </div>
            <h2>Order Items</h2>
            {orderItems.length > 0 ? (
                orderItems.map(item => (
                    <div key={item.orderItemId} className='order-items-container'>

                        <ul>
                            <li><strong>Product Id:</strong> {item.productId}</li>
                            <li><strong>Quantity:</strong> {item.quantity}</li>
                            <li><strong>Price:</strong> {item.price}</li>
                        </ul>
                        <button className='delete-button' onClick={() => openDeleteModal(item)}>Delete</button>
                    </div>
                ))
            ) : (
                <p>No items found for this order</p>
            )}
            <button className='send-order-button' onClick={(e) => { handleSubmitForm(e); handleConfirmUpdate() }}>Send the order</button>
            <ToastContainer />
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDelete}
                onConfirm={handleDeleteItem}
                title="Confirm Delete"
                message="Are you sure you want to delete this item?"
                confirmButtonText="Delete"
                confirmButtonClass="delete"
            />
            <ConfirmModal
                isOpen={showUpdateModal}
                onCancel={cancelUpdate}
                onConfirm={handleConfirmUpdate}
                title="Confirm Update"
                message="Are you sure you want to update this item?"
                confirmButtonText="Update"
                confirmButtonClass="update"
            />
        </div>
    );
}

export default AdminUpdateOrder