import React from 'react'
import { useState, useEffect } from 'react';
import API_BASE_URL from "../../config";
import Items from '../Items/Items';

const ProductsDisplay = () => {

    const [products, setProducts] = useState("");

    useEffect(() => {
        async function fetchProducts() {
            try {
                const response = await fetch(`${API_BASE_URL}/product/available`);
                const data = await response.json();
                setProducts(data);

            } catch (error) {
                console.error("Error fetching products:", error);
            }
        }

        fetchProducts();
    }, []);


    return (
        <div className='produtsDisplay'>

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

export default ProductsDisplay;

