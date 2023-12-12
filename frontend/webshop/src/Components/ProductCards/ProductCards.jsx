import React from "react";
import { useNavigate } from "react-router-dom";
import "./ProductCards.css";
import API_BASE_URL from "../../config";

const ProductCard = ({ product, isLoggedIn }) => {

    const navigate = useNavigate();
    const { id } = product;
    const onView = () => {
        navigate(`/product/${id}?isLoggedIn=${isLoggedIn}`);
    }
    return (
        <div class="card" key={id}>

            <div className="imgBox">
                <img
                    className="product-img"
                    src={`data:image/jpeg;base64,${product.imageBase64}`}
                    alt={product.name}
                />
            </div>

            <div className="contentBox" >
                <h3>
                    <span>{product.productname}</span>
                </h3>
                <h4>{product.category}</h4>
                <h2>{product.price} $</h2>
                <h2>{product.stock}</h2>
                <a href="#" class="buy">Add To Card</a>
                <button className="product-view-btn" onClick={onView}>
                    View Product
                </button>
            </div>


        </div>
    )
}

export default ProductCard;
