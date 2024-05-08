import React from 'react'
import './Items.css'


const Items = (props) => {
    const bonus = props.discount ? props.price * (props.discount / 100) : 0;

    const newPrice = Math.ceil(props.price - bonus);


    const priceStyle = {
        color: props.discount ? 'red' : 'black'
    };
    return (
        <React.Fragment key={props.productId}>
            <div className='item_container'>
                <div className='imgBox'>
                    <img src={props.imageBase64}
                        alt={props.productName} />

                </div>


                <div className="content_box">
                    <h3>
                        <span>{props.productName}</span>
                    </h3>
                    <hr />
                    <h4 style={priceStyle}>Price : {newPrice}$</h4>
                    <hr />
                    <h5>Stock : {props.stock}db</h5>
                    <hr />
                    <h5>{props.category === 1 ? "Dog" : "Cat"}</h5>
                    <hr />
                    <h5>{props.subCategory}</h5>
                    <hr />
                    {props.discount > 0 && <h4>Discount : {props.discount * 100}%</h4>}
                    <hr />
                    <button>View</button>
                </div>

            </div>
        </React.Fragment>

    )
}

export default Items;




