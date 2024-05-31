import React from 'react'
import { useState, useEffect } from 'react';
import API_BASE_URL from "../../config";
import Items from '../Items/Items';
import './ProductsDisplay.css'


const ProductsDisplay = ({ onlyDiscounted = false }) => {

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

    const filteredProducts = onlyDiscounted ? products.filter(product => product.discount > 0) : products

    return (
        <div className="produtsDisplay">

            {filteredProducts.length > 0 ? (
                <div className="data">
                    {filteredProducts.map((product, i) => (
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

