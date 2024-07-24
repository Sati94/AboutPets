import React, { useEffect, useState, useContext } from 'react'
import API_BASE_URL from '../../config'
import { useNavigate } from 'react-router-dom'
import { AuthContext } from '../../AuthContext/AuthContext'
import ConfirmModal from '../../Modal/ConfimModal'
import SearchInput from '../../Components/SearchInput/SearchInput'
import { ToastContainer, toast } from 'react-toastify'
import './AdminOrders.css'
const AdminOrders = () => {

    const { authState } = useContext(AuthContext);
    const [orders, setOrders] = useState([]);
    const [deleteToOrder, setDeleteToOrder] = useState(null);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredOrders, setFilteredOrders] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [orderId, setOrderId] = useState(0);
    const navigate = useNavigate();

    const statusMapping = {
        1: 'Pending',
        2: 'Processing',
        3: 'Shipped',
        4: 'Delivered',
        5: 'Cancelled'

    }
    const deliveryMapping = {
        1: 'GLS',
        2: 'Post',
        3: 'DPD'
    }

    const formatDate = (createdDate) => {
        const date = new Date(createdDate);
        const year = date.getFullYear();
        const month = ('0' + (date.getMonth() + 1)).slice(-2);
        const day = ('0' + date.getDate()).slice(-2);
        const hours = ('0' + date.getHours()).slice(-2);
        const minutes = ('0' + date.getMinutes()).slice(-2);

        return `${year}-${month}-${day} ${hours}:${minutes}`;
    };
    useEffect(() => {
        async function fetchOrders() {
            try {
                const { token, role } = authState;
                const response = await fetch(`${API_BASE_URL}/orderlist/all`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                })
                const data = await response.json();
                setOrders(data);
                console.log(data)
            } catch (error) {
                console.log("Falied fetch orders..")
            }

        }
        if (authState.token) {
            fetchOrders();
        }

    }, [authState.token, authState.role]);

    useEffect(() => {
        const data = orders.filter(item => {
            const searchLower = searchTerm.toLowerCase();
            const statusText = statusMapping[item.orderStatuses]?.toLowerCase();
            return statusText && statusText.includes(searchLower);

        });
        setFilteredOrders(data);


    }, [searchTerm, orders]);

    const handleSearch = (term) => {
        setSearchTerm(term)
    };

    const toggleDeleteModal = () => {
        setShowDeleteModal(!showDeleteModal);
    }
    const openDeleteModal = (order) => {
        setDeleteToOrder(order);

        setShowDeleteModal(true);
    }
    const cancelDelete = () => {
        setShowDeleteModal(false);
    }
    const handleOrderElement = (orderId) => {
        setOrderId(orderId);
        navigate(`/admin/orders/${orderId}`)
    };

    const confirmToDelete = async () => {
        if (deleteToOrder) {
            try {
                const { role, token } = authState;
                const response = await fetch(`${API_BASE_URL}/order/delete/${deleteToOrder.orderId}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                })
                if (response.ok) {
                    toast.success("Element deleted!");
                    const updatedOrders = orders.filter(item => item.orderId !== deleteToOrder.orderId);
                    setFilteredOrders(updatedOrders);
                    setOrders(updatedOrders);
                    setShowDeleteModal(false);
                }
                else {
                    toast.error("Somehing is worng!");
                }
            }
            catch (error) {
                console.log("Error:", error);
                toast.error("Somehing is worng!")
            }
        }
    };



    return (
        <div className='order-container'>
            <h1>Orders</h1>
            <div className="search-bar text-center">
                <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Orders..." />
            </div>
            <div className='order-list'>
                {filteredOrders.map(order => (


                    <div key={order.orderId} className='order-item-data'>
                        <ul>
                            <li><strong>Id:</strong> {order.orderId}</li>
                            <li><strong>Date:</strong> {formatDate(order.orderDate)}</li>
                            <li><strong>Country:</strong> {order.country}</li>
                            <li><strong>City:</strong> {order.city}</li>
                            <li><strong>Street Address:</strong> {order.streetAddress}</li>
                            <li><strong>Delivery Type:</strong> {deliveryMapping[order.deliveryType]}</li>
                            <li><strong>Tortal Price:</strong>{order.totalPrice}$</li>
                            <li><strong>Status:</strong> {statusMapping[order.orderStatuses]}</li>
                            <li><strong>User Id:</strong>{order.userId}</li>
                        </ul>
                        <div className='buttons-actions'>
                            <button className='update' onClick={() => handleOrderElement(order.orderId)}> Update</button>
                            <button className='delete' onClick={() => openDeleteModal(order)}>Delete</button>
                        </div>

                    </div>



                ))}
            </div>
            <ToastContainer />
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDelete}
                onConfirm={confirmToDelete}
                title="Confirm Delete"
                message="Are you sure you want to delete this order?"
                confirmButtonText="Delete"
                confirmButtonClass="delete" />

        </div >
    )

}

export default AdminOrders