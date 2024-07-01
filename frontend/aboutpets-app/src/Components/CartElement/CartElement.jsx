import React, { useContext } from 'react'
import "./CartElement.css";
import { useState, useEffect } from 'react';
import API_BASE_URL from '../../config';
import { ToastContainer, toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../../AuthContext/AuthContext';
import Cookies from 'js-cookie';


const CartElement = () => {

    const [orderItems, setOrderItems] = useState([]);
    const [orders, setOrders] = useState([]);
    const { authState, setAuthState } = useContext(AuthContext)
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {

        const fetchOrderById = async () => {

            try {
                const { token, role, orderId } = authState;

                console.log(orderId)
                const response = await fetch(`${API_BASE_URL}/order/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    },
                })
                if (!response.ok) {
                    throw new Error('Failed to fetch the data!');
                }
                const data = await response.json();
                console.log(data);
                setOrders(data);
            } catch (error) {
                console.error("Error fetching orders:", error);
            }

        }
        if (authState.token) {
            fetchOrderById();
        }

    }, [authState.token, loading])



    useEffect(() => {
        const { token, role, orderId } = authState;

        const fetchOrderItems = async () => {
            try {

                const response = await fetch(`${API_BASE_URL}/order/orderItems/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    },
                })
                if (!response.ok) {
                    throw new Error('Failed to fetch the data!');
                }
                const data = await response.json();
                console.log(data);
                setOrderItems(data);

            } catch (error) {
                console.error("Error fetching orderItems:", error);

                setAuthState(prevState => ({
                    ...prevState,
                    orderId: null
                }));
            }


        };
        if (authState.token && authState.orderId) {
            fetchOrderItems();
        }
    }, [authState.token, loading]);


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
                // Ha a válasz hibás, dobunk egy hibát
                const errorMessage = await response.text();
                throw new Error(`Failed to remove order item: ${errorMessage}`);
            }

            // A válasz megfelelő, a megrendelési tétel sikeresen eltávolítva
            console.log('Order item removed successfully!');
            toast.success("Item deleted!")
            setLoading((prev) => !prev);
        } catch (error) {
            navigate("/")
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
            console.log('Order status updated successfully!');
            toast.success('Order is sending!')

            setLoading((prev) => !prev);



            navigate("/")
        } catch (error) {
            console.error('Error updating order status:', error.message);
        }
    };

    return (
        <div>
            {orderItems.length === 0 || orders.orderStatuses > 1 || orderItems === null ? (
                <div className='No-Data'>
                    <p>No data</p>
                </div>
            ) : (
                <ul>
                    {orderItems.map((item) => (
                        <li key={item.orderItemId} className="cart-item"> {/* Hozzáadva a cart-item class */}
                            <div className="cart-item-content">
                                <img src={`data:image/jpeg;base64,${item.product.imageBase64}`} alt={item.product.productName} className="cart-item-image" /> {/* Kép hozzáadása */}
                                <div className="cart-item-details">
                                    <p>{item.product.productName}</p>
                                    <p>{item.product.description}</p>
                                    <p>Price: {item.price}</p>
                                    <p>Stock: {item.quantity}</p>

                                </div>
                                <button onClick={() => removedOrderItem(item.orderItemId)}>Delete</button>
                            </div>

                        </li>
                    ))}
                    <h2 className='Total-Price'>Total Price : {orders.totalPrice}</h2>
                    <button className='Send-Order-Button' onClick={updateOrderStatus}>Send the order</button>


                </ul>
            )}
            <ToastContainer />
        </div>

    )
}

export default CartElement