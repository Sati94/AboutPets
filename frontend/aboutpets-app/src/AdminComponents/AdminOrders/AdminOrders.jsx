import React, { useEffect, useState, useContext } from 'react'
import API_BASE_URL from '../../config'
import { useNavigate } from 'react-router-dom'
import { AuthContext } from '../../AuthContext/AuthContext'
import DeleteConfirmModal from '../../Modal/DeleteConfirmModal'
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
        <div>
            <h1>Orders</h1>
            <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Order Statuses..." />
            <div className='order-list'>
                {filteredOrders.map(order => (


                    <div key={order.orderId} className='order-item'>
                        <ul>
                            <li><strong>Id:</strong> {order.orderId}</li>
                            <li><strong>Date:</strong> {order.orderDate}</li>
                            <li><strong>Tortal Price:</strong>{order.totalPrice}</li>
                            <li><strong>Status:</strong> {statusMapping[order.orderStatuses]}</li>
                            <li><strong>User Id:</strong>{order.userId}</li>
                        </ul>
                        <button className='update' onClick={() => handleOrderElement(order.orderId)}> Update</button>
                        <button className='delete' onClick={() => openDeleteModal(order)}>Delete</button>

                    </div>



                ))}
            </div>
            <ToastContainer />
            <DeleteConfirmModal isOpen={showDeleteModal} onCancel={cancelDelete} onConfirm={confirmToDelete} />

        </div >
    )

}

export default AdminOrders