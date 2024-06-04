import React from 'react'
import { useEffect, useState } from 'react'
import API_BASE_URL from '../config'
import Items from '../Components/Items/Items'
import { useParams } from 'react-router-dom'
import SearchInput from '../Components/SearchInput/SearchInput'
import '../Components/SearchInput/SearchInput.css'



const SubCategoryPage = () => {

    const { category, subCategory } = useParams();
    const [products, setProducts] = useState([]);
    const [searchTerm, setSearchTerm] = useState("");
    const [filteredProducts, setFilteredProducts] = useState([]);

    useEffect(() => {

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


    }, [category, subCategory]);


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

    return (
        <div className='products'>
            <div className="search-input-container">
                <SearchInput onSearch={handleSearch} />
            </div>
            {products.length > 0 ? (
                <div className='data'>
                    {filteredProducts.map((product) => (
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

export default SubCategoryPage