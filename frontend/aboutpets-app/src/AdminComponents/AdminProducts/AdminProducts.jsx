import React from 'react'
import { useEffect, useContext, useState } from 'react'
import './AdminProducts.css'
import { AuthContext } from '../../AuthContext/AuthContext'
import SearchInput from '../../Components/SearchInput/SearchInput'
import API_BASE_URL from '../../config'
import { Link, useNavigate } from 'react-router-dom'

const AdminProducts = () => {
    const { authState } = useContext(AuthContext);
    const [products, setProducts] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredProducts, setFilteredProducts] = useState([]);
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


    }, []);



    useEffect(() => {
        const data = products.filter(product => {
            const searchLower = searchTerm.toLowerCase();

            return (
                product.productName.toLowerCase().includes(searchLower)
            )

        });
        setFilteredProducts(data);

    }, [searchTerm, products]);

    const handleSearch = (term) => {
        setSearchTerm(term);
    }

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
    const handleClick = (product) => {
        navigate(`/admin/product/${product.productId}`)

    }
    return (
        <div>
            <h1>Products</h1>
            <SearchInput value={searchTerm} onSearch={handleSearch} placeholder="Search Products..." />
            <button >Add Product</button>
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
                                <li><strong>Image:</strong> {product.imageBase64}</li>
                            </ul>
                            <button className='update' onClick={() => handleClick(product)}>Update</button>
                            <button className='delete' >Delete</button>
                        </div>
                    )
                })}
            </div>
        </div>
    )
}

export default AdminProducts