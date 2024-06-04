import React from 'react'
import "./SearchInput.css"
import { useState } from 'react'

const SearchInput = ({ onSearch }) => {

    const [searchTerm, setSearchTerm] = useState("");

    const handleSearch = (e) => {
        const inputText = e.target.value

        setSearchTerm(inputText);
        console.log("Search term: ", inputText)
        onSearch(inputText)


    }


    return (
        <input
            className="search-input"
            type="text"
            placeholder="Search"
            onChange={handleSearch}

        />
    );
};

export default SearchInput