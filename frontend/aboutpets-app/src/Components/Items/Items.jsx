import React from 'react'
import './Items.css'
import { useNavigate } from 'react-router-dom';


const Items = (props) => {
    const navigate = useNavigate();

    const handleClick = () => {
        navigate(`/products/${props.productId}`)
    }
    const bonus = props.discount ? props.price * (props.discount / 100) : 0;


    const newPrice = Math.ceil(props.price - bonus);


    const priceStyle = {
        color: props.discount ? 'red' : 'black'


    };

    const containerClass = props.stock === 0 ? 'item_container out-of-stock' : 'item_container'
    return (

        <div className={containerClass} key={props.productId}>
            <div className='imgBox'>
                <img src={`data:image/jpeg;base64,${props.image}`}
                    alt={props.productName} />

            </div>


            <div className="content_box">
                <h3>
                    <span>{props.productName}</span>
                </h3>
                <hr />
                <h4 style={priceStyle}>Price : {newPrice}$</h4>
                <hr />
                <h5>Stock : {props.stock}pcs</h5>
                <hr />
                <h5>{props.category === 1 ? "Dog" : "Cat"}</h5>
                <hr />
                <h5>{props.subCategory}</h5>
                <hr />
                {props.discount > 0 && <h4>Discount : {props.discount * 100}%</h4>}
                <hr />
                <button onClick={handleClick} disabled={props.stock === 0}>View</button>
            </div>

        </div>


    )
}

export default Items;




