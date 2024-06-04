import React from 'react'
import { useState, useEffect } from 'react';
import API_BASE_URL from "../../config";
import Items from '../Items/Items';
import './ProductsDisplay.css'
import SearchInput from '../SearchInput/SearchInput';


const ProductsDisplay = ({ onlyDiscounted = false }) => {

    const [products, setProducts] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredProducts, setFilteredProducts] = useState([]);

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

    }, []
    );

    const handleSearch = (term) => {
        setSearchTerm(term);
    }

    useEffect(() => {
        const data = products.filter(product => {
            const searchLower = searchTerm.toLowerCase();

            return (
                product.productName.toLowerCase().includes(searchLower)
            )

        });
        setFilteredProducts(data);

    }, [searchTerm, products]);


    const finalProducts = onlyDiscounted ? filteredProducts.filter(product => product.discount > 0) : filteredProducts



    return (

        <div className="produtsDisplay">
            <div className="search-input-container ">
                <SearchInput onSearch={handleSearch} />
            </div>
            {filteredProducts.length > 0 ? (
                <div className="data">
                    {finalProducts.map((product) => (
                        <Items
                            key={product.productId}
                            productId={product.productId}
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

