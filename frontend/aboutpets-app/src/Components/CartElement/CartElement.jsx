import React from 'react'
import "./CartElement.css";
import Cookies from 'js-cookie';
import { useState, useEffect } from 'react';
import API_BASE_URL from '../../config';
import { ToastContainer, toast } from 'react-toastify';
import { useNavigate } from 'react-router-dom';

const CartElement = () => {

    const [orderItems, setOrderItems] = useState([]);
    const [orders, setOrders] = useState([]);
    const userToken = Cookies.get("userToken");
    const userRole = Cookies.get("Role");
    const orderId = Cookies.get("orderId");
    const userId = Cookies.get("userId");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchOrderById = async () => {
            try {

                console.log(orderId)
                const response = await fetch(`${API_BASE_URL}/order/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${userToken}`,
                        'Role': userRole
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
        fetchOrderById()
    }, [loading])



    useEffect(() => {
        const fetchOrderItems = async () => {
            try {

                const response = await fetch(`${API_BASE_URL}/order/orderItems/${orderId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${userToken}`,
                        'Role': userRole
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
            }


        };
        fetchOrderItems();
    }, [loading]);

    /*const groupedOrderItems = orderItems.reduce((acc, item) => {
        const existingItemIndex = acc.findIndex(i => i.productId === item.productId);
        if (existingItemIndex !== -1) {

            acc[existingItemIndex].quantity += item.quantity;
            acc[existingItemIndex].price += item.price;
        } else {

            acc.push(item);
        }
        return acc;
    }, []);*/

    const removedOrderItem = async (orderItemId) => {
        try {

            const response = await fetch(`${API_BASE_URL}/orderitem/remove?orderId=${orderId}&orderItemId=${orderItemId}&userId=${userId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${userToken}`,
                    'Role': userRole
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
            setLoading(!loading);
        } catch (error) {
            navigate("/")
            console.error('Error removing order item:', error.message);
        }

    };

    const updateOrderStatus = async () => {
        try {
            const response = await fetch(`${API_BASE_URL}/order/update/${orderId}?&orderstatus=2`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${userToken}`,
                    'Role': userRole
                },
            });
            if (!response.ok) {
                const errorMessage = await response.text();
                throw new Error(`Failed to update order status: ${errorMessage}`);
            }
            console.log('Order status updated successfully!');
            toast.success('Order is sending!')

            setLoading(!loading);
            Cookies.remove("orderId")
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
                                <img src={item.product.imageBase64} alt={item.product.productName} className="cart-item-image" /> {/* Kép hozzáadása */}
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