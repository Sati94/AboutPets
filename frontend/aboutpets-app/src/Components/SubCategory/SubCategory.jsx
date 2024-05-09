import React from 'react'
import './SubCategory.css'
import accessories from '../Assets/accessories.png'
import dryfood from '../Assets/dryfood.png'
import games from '../Assets/games.png'
import wetfood from '../Assets/wetfood.png'

const SubCategory = () => {
    return (

        <div className='subcategory-card'>
            <div className='subcategory-item'>
                <img src={dryfood} alt="Dryfood" />
                <button>DryFood</button>
            </div>
            <div className='subcategory-item'>
                <img src={wetfood} alt="Wetfood" />
                <button>WetFood</button>
            </div>
            <div className='subcategory-item'>
                <img src={games} alt="Games" />
                <button>Games</button>
            </div>
            <div className='subcategory-item'>
                <img src={accessories} alt="Accessories" />
                <button>Accessories</button>
            </div>
        </div>
    )
}

export default SubCategory