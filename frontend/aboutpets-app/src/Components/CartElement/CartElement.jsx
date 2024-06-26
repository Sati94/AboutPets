import React, { useContext, useState, useEffect } from 'react';
import "./CartElement.css";
import API_BASE_URL from '../../config';
import { ToastContainer, toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../AuthContext/AuthContext';
import ConfirmModal from '../../Modal/ConfimModal';


const CartElement = () => {
    const [orderItems, setOrderItems] = useState([]);
    const [orders, setOrders] = useState({});
    const { authState, setAuthState } = useContext(AuthContext);
    const [loading, setLoading] = useState(false);
    const [hasBonus, setHasBonus] = useState(false);
    const [showApplyBonusModal, setShowApplyBonusModal] = useState(false);
    const [showSendOrderModal, setShowSendOrderModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [deleteItemId, setDeleteItemId] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchOrderById = async () => {
            try {
                const { token, role, orderId } = authState;

                if (!orderId) {
                    return;
                }

                const response = await fetch(`${API_BASE_URL}/order/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch the data!');
                }

                const data = await response.json();
                setOrders(data);
            } catch (error) {
                console.error("Error fetching orders:", error);
            }
        };

        const fetchUserProfile = async () => {
            try {
                const { token, userId } = authState;

                const response = await fetch(`${API_BASE_URL}/UserProfile/user/profile/${userId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                    },
                });

                if (!response.ok) {
                    throw new Error('Failed to fetch user profile!');
                }

                const userProfile = await response.json();
                console.log(userProfile)
                if (userProfile.bonus > 0) {
                    setHasBonus(true);
                } else {
                    setHasBonus(false);
                }
            } catch (error) {
                console.error("Error fetching user profile:", error);
            }
        };

        if (authState.token) {
            fetchOrderById();
            fetchUserProfile();
        }
    }, [authState.token, loading, authState.orderId]);


    useEffect(() => {
        const fetchOrderItems = async () => {
            const { token, role, orderId } = authState;

            if (!orderId) {
                return;
            }

            const response = await fetch(`${API_BASE_URL}/order/orderItems/${orderId}`, {
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
            });

            const data = await response.json();
            setOrderItems(data);

            // If no order items, set orderId to null
            if (data.length === 0) {
                setAuthState(prevState => ({
                    ...prevState,
                    orderId: null
                }));
            }
        };

        if (authState.token && authState.orderId) {
            fetchOrderItems();
        }
    }, [authState.token, loading, authState.orderId, setAuthState]);

    const removedOrderItem = async (orderItemId) => {
        const { orderId, token, role, userId } = authState;

        try {
            const response = await fetch(`${API_BASE_URL}/orderitem/remove?orderId=${orderId}&orderItemId=${orderItemId}&userId=${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Failed to remove order item: ${errorMessage}`);
            }

            toast.success("Item deleted!");
            setLoading(prev => !prev);
            const updatedOrderItems = orderItems.filter(item => item.orderItemId !== orderItemId);
            setOrderItems(updatedOrderItems);
        } catch (error) {
            console.error('Error removing order item:', error.message);
        }
    };

    const updateOrderStatus = async () => {
        const { orderId, token, role } = authState;
        try {
            const response = await fetch(`${API_BASE_URL}/order/update/${orderId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(2)
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Failed to update order status: ${errorMessage}`);
            }

            toast.success('Order is sending!');
            setLoading(prev => !prev);
            setAuthState(prevState => ({
                ...prevState,
                orderId: null
            }));
            navigate("/");
        } catch (error) {
            console.error('Error updating order status:', error.message);
        }
    };
    const applyBonusToOrder = async () => {
        const { orderId, token, userId } = authState;
        try {
            const response = await fetch(`${API_BASE_URL}/order/${orderId}/apply-cupon/${userId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Failed to apply bonus to order: ${errorMessage}`);
            }

            toast.success('Bonus applied to order!');
            setLoading(prev => !prev);
            // You may want to refetch order details or update state after applying bonus
        } catch (error) {
            console.error('Error applying bonus to order:', error.message);
        }
    };

    const toggleApplyBonusModal = () => {
        setShowApplyBonusModal(!showApplyBonusModal);
    };

    const toggleSendOrderModal = () => {
        setShowSendOrderModal(!showSendOrderModal);
    };

    const toggleDeleteModal = () => {
        setShowDeleteModal(!showDeleteModal);
    };

    const confirmApplyBonus = async () => {
        toggleApplyBonusModal(); // Close modal
        await applyBonusToOrder(); // Apply bonus
    };

    const confirmSendOrder = async () => {
        toggleSendOrderModal(); // Close modal
        await updateOrderStatus(); // Send order
    };

    const confirmDeleteItem = async () => {
        toggleDeleteModal(); // Close modal
        await removedOrderItem(deleteItemId); // Remove item
    };

    const cancelApplyBonus = () => {
        setShowApplyBonusModal(false);
    };

    const cancelSendOrder = () => {
        setShowSendOrderModal(false);
    };

    const cancelDeleteItem = () => {
        setShowDeleteModal(false);
    };
    return (
        <div>
            {!Array.isArray(orderItems) || orderItems.length === 0 || orders.orderStatuses > 1 ? (
                <div className='No-Data'>
                    <p>No data</p>
                </div>
            ) : (
                <ul>
                    {orderItems.map((item) => (
                        <li key={item.orderItemId} className="cart-item">
                            <div className="cart-item-content">
                                <img src={`data:image/jpeg;base64,${item.product.imageBase64}`} alt={item.product.productName} className="cart-item-image" />
                                <div className="cart-item-details">
                                    <p>{item.product.productName}</p>
                                    <p>{item.product.description}</p>
                                    <p>Price: {item.price}</p>
                                    <p>Stock: {item.quantity}</p>
                                </div>
                                <button onClick={() => {
                                    setDeleteItemId(item.orderItemId);
                                    setShowDeleteModal(true);
                                }}>Delete</button>
                            </div>
                        </li>
                    ))}
                    <h2 className='Total-Price'>Total Price : {orders.totalPrice}</h2>
                    {hasBonus && <button className='Apply-Bonus-Button' onClick={toggleApplyBonusModal}>Apply Bonus</button>}
                    <button className='Send-Order-Button' onClick={toggleSendOrderModal}>Send the order</button>

                </ul>
            )}
            <ToastContainer />
            <ConfirmModal
                isOpen={showApplyBonusModal}
                onCancel={cancelApplyBonus}
                onConfirm={confirmApplyBonus}
                title="Confirm Apply Bonus"
                message="Are you sure you want to apply bonus to this order?"
                confirmButtonText="Apply Bonus"
                confirmButtonClass="add"
            />
            {/* Confirm modal for Send Order */}
            <ConfirmModal
                isOpen={showSendOrderModal}
                onCancel={cancelSendOrder}
                onConfirm={confirmSendOrder}
                title="Confirm Send Order"
                message="Are you sure you want to send this order?(If you have a bonus, have you activated that?)"
                confirmButtonText="Send Order"
                confirmButtonClass="send"
            />
            {/* Confirm modal for Delete Item */}
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDeleteItem}
                onConfirm={confirmDeleteItem}
                title="Confirm Delete"
                message="Are you sure you want to delete this item from the order?"
                confirmButtonText="Delete"
                confirmButtonClass="delete"
            />
        </div>
    );
}

export default CartElement;