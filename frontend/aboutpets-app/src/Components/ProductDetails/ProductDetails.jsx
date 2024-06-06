import React from 'react'
import "./ProductDetails.css"
import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom'
import API_BASE_URL from '../../config'
import { useNavigate } from 'react-router-dom'
import { ToastContainer, toast } from 'react-toastify'
import Cookies from 'js-cookie'
import 'react-toastify/ReactToastify.css'

const ProductDetails = () => {

    const { productId } = useParams();
    const [product, setProduct] = useState([]);
    const [quantity, setQuantity] = useState(1);
    const [orderId, setOrderId] = useState(null);
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);



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
    }, [productId, loading]);


    const handleAddToCart = async () => {

        const userId = Cookies.get("userId");
        if (!userId) {

            navigate("/login", { state: { message: "You have to Log In First" } });
        }
        try {

            const url = new URL(`${API_BASE_URL}/add`);
            url.searchParams.append('userId', userId);
            url.searchParams.append('productId', productId);
            url.searchParams.append('quantity', quantity);
            url.searchParams.append('orderId', orderId || 0);

            const response = await fetch(url, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${Cookies.get('userToken')}`,
                    'Role': Cookies.get('userRole')
                },
                body: JSON.stringify({
                    userId,
                    productId: parseInt(productId),
                    quantity,
                    orderId: orderId || 0
                })
            });
            if (!response.ok) {
                toast.error('Failed to add item to cart!')
                throw new Error('Failed to add item to cart!')
            }
            const data = await response.json();
            toast.success('Item added to cart!')
            console.log(data);
        } catch (error) {

            console.error("Error adding item to cart!");

        }
        setOrderId(orderId);
        console.log(orderId)
        setLoading(!loading);
    }

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
            <div className="quantity">
                <button onClick={() => setQuantity(Math.max(quantity - 1, 1))}>-</button>
                <span>{quantity}</span>
                <button onClick={() => setQuantity(Math.min(quantity + 1, product.stock))}>+</button>
            </div>
            <button onClick={handleAddToCart}>Add to Cart</button>
            <ToastContainer />
        </div>
    )
}

export default ProductDetails