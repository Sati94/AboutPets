import React from 'react'
import { useEffect, useState } from 'react'
import API_BASE_URL from '../config'
import Items from '../Components/Items/Items'


const SubCategoryPage = ({ category, subCategory }) => {

    const [products, setProducts] = useState([]);
    useEffect(() => {
        if (category && subCategory) {
            async function fetchProducts() {
                try {
                    const response = await fetch(`${API_BASE_URL}/products/${category}/${subCategory}`);
                    const data = await response.json();
                    setProducts(data);

                } catch (error) {
                    console.error("Error fetching products:", error);
                }
            }

            fetchProducts();
        }

    }, [category, subCategory]);
    return (
        <div className='products'>

            {products.length > 0 ? (
                <div className='data'>
                    {products.map((product, i) => (
                        <Items
                            key={i}
                            id={product.productId}
                            productName={product.productName}
                            stock={product.stock}
                            price={product.price}
                            category={product.category}
                            subCategory={product.subCategory}
                            discount={product.discount}
                            image={product.imageBase64}
                        />
                    ))}
                </div>
            ) : (
                <>No Product added yet...</>
            )}

        </div>
    )
}

export default SubCategoryPage