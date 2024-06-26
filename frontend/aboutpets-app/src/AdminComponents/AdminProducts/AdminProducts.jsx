import React from 'react'
import { useEffect, useContext, useState } from 'react'
import './AdminProducts.css'
import { AuthContext } from '../../AuthContext/AuthContext'
import SearchInput from '../../Components/SearchInput/SearchInput'
import API_BASE_URL from '../../config'
import { Link, useNavigate } from 'react-router-dom'
import { ToastContainer, toast } from 'react-toastify'
import ConfirmModal from '../../Modal/ConfimModal'

const AdminProducts = () => {
    const { authState } = useContext(AuthContext);
    const [products, setProducts] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [productToDelete, setProductToDelete] = useState(false);
    const navigate = useNavigate();


    useEffect(() => {

        async function fetchProducts() {
            const { token } = authState
            try {
                const response = await fetch(`${API_BASE_URL}/product/available`)

                const data = await response.json();
                setProducts(data);

            } catch (error) {
                console.error("Error fetching products:", error);
            }
        }
        if (authState.token) {
            fetchProducts();
        }


    }, [authState]);

    const handleSearch = (term) => {
        setSearchTerm(term);
    }
    const handleDelete = async (productId) => {

        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/product/delete/${productId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                }
            })
            if (response.ok) {
                toast.success("Element deleted");
                const updatedProducts = products.filter((product) => product.productId !== productId);
                setProducts(updatedProducts);
                setFilteredProducts(updatedProducts);
            }
            else {
                toast.error("Somehing is worng!");
            }

        }
        catch (error) {
            console.log("Error:", error);
            toast.error("Somehing is worng!")
        }

    };

    const handleClick = (product) => {
        navigate(`/admin/product/${product.productId}`)

    }
    const addProductClick = () => {
        navigate("/admin/addproduct")
    }
    const toggleDeleteModal = () => {
        setShowDeleteModal(!showDeleteModal);
    }
    const openDeleteModal = (product) => {
        setProductToDelete(product);
        setShowDeleteModal(true);
    }

    const confirmDelete = async () => {
        if (productToDelete) {
            await handleDelete(productToDelete.productId);
            setShowDeleteModal(false);
        }
    }
    const cancelDelete = () => {
        setShowDeleteModal(false);
    }

    useEffect(() => {
        const data = products.filter(product => {
            const searchLower = searchTerm.toLowerCase();

            return (
                product.productName.toLowerCase().includes(searchLower)
            )

        });
        setFilteredProducts(data);

    }, [searchTerm, products]);



    const categoryMapping = {
        1: 'Dog',
        2: 'Cat'
    };

    const subCategoryMapping = {
        1: 'Games',
        2: 'Accessories',
        3: 'WetFood',
        4: 'DryFood'
    };




    return (
        <div>
            <h1>Products</h1>
            <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Products..." />
            <button onClick={addProductClick}>Add Product</button>
            <div className='product-list'>
                {filteredProducts.map(product => {
                    return (
                        <div key={product.productId} className='product-item'>
                            <ul>
                                <li><strong>Product Name:</strong> {product.productName}</li>
                                <li><strong>Description:</strong> {product.description}</li>
                                <li><strong>Price:</strong> {product.price}</li>
                                <li><strong>Stock:</strong> {product.stock}</li>
                                <li><strong>Discount:</strong> {product.discount}</li>
                                <li><strong>Category:</strong> {categoryMapping[product.category]}</li>
                                <li><strong>SubCategory:</strong> {subCategoryMapping[product.subCategory]}</li>
                                {product.imageBase64 ?
                                    (<li><strong>Image:</strong> Yes </li>) : (
                                        <li><strong>Image:</strong> No </li>
                                    )}

                            </ul>
                            <button className='update' onClick={() => handleClick(product)}>Update</button>
                            <button className='delete' onClick={() => openDeleteModal(product)}>Delete</button>
                        </div>
                    )
                })}
            </div>
            <ToastContainer />
            <ConfirmModal
                isOpen={showDeleteModal}
                onCancel={cancelDelete}
                onConfirm={confirmDelete}
                title="Confirm Delete"
                message="Are you sure you want to delete this product?"
                confirmButtonText="Delete"
                confirmButtonClass="delete"
            />
        </div>
    )
}

export default AdminProducts