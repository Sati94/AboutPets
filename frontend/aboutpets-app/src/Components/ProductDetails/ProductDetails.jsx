import React from 'react'
import "./ProductDetails.css"
import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import API_BASE_URL from '../../config'


const ProductDetails = () => {

    const { productId } = useParams();
    const [product, setProduct] = useState([]);


    useEffect(() => {
        async function fetchProducts() {
            try {
                const response = await fetch(`${API_BASE_URL}/product/${productId}`);
                const data = await response.json();
                setProduct(data);

            } catch (error) {
                console.error("Error fetching products:", error);
            }
        }

        fetchProducts();
    }, [productId]);

    if (!product) {
        return <div>Loading...</div>;
    }

    return (
        <div className="productDetails">
            <img src={product.imageBase64} alt={product.productName} />
            <h1>{product.productName}</h1>
            <p>Price: ${product.price}</p>
            {product.discount > 0 && <p>Discount: {product.discount * 100}%</p>}
            <p>Stock: {product.stock} pcs</p>
            <p>Description: {product.description}</p>
            <button>Add to Cart</button>
        </div>
    )
}

export default ProductDetails