import React from 'react'
import './SubCategory.css'
import accessories from '../Assets/accessories.png'
import dryfood from '../Assets/dryfood.png'
import games from '../Assets/games.png'
import wetfood from '../Assets/wetfood.png'
import { useNavigate } from 'react-router-dom'


const SubCategory = ({ category }) => {
    const navigate = useNavigate();
    const handleClick = (subCategory) => {

        return navigate(`/category/${category}/${subCategory}`);
    }

    return (

        <div className='subcategory-card'>
            <div className='subcategory-item'>
                <img src={dryfood} alt="Dryfood" />
                <button onClick={() => handleClick(4)}>DryFood</button>
            </div>
            <div className='subcategory-item'>
                <img src={wetfood} alt="Wetfood" />
                <button onClick={() => handleClick(3)}>WetFood</button>
            </div>
            <div className='subcategory-item'>
                <img src={games} alt="Games" />
                <button onClick={() => handleClick(1)}>Games</button>
            </div>
            <div className='subcategory-item'>
                <img src={accessories} alt="Accessories" />
                <button onClick={() => handleClick(2)}>Accessories</button>
            </div>
        </div>
    )
}

export default SubCategory