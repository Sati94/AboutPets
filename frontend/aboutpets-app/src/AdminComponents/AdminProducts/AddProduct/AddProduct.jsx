import React from 'react'
import { useState, useContext, useEffect } from 'react'
import API_BASE_URL from '../../../config'
import { AuthContext } from '../../../AuthContext/AuthContext'
import { useNavigate } from 'react-router-dom'




const AddProduct = () => {

    const { authState } = useContext(AuthContext);
    const navigate = useNavigate();
    const [product, setProduct] = useState({
        productName: "",
        description: "",
        price: "",
        stock: "",
        discount: "",
        category: "",
        subCategory: "",
        imageBase64: ""
    });

    const [selectedFile, setSelectedFile] = useState(null);
    const readFileAsBase64AndCompress = (file, maxWidth = 800, maxHeight = 600, quality = 0.8) => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => {
                const originalImageBase64 = reader.result.split(",")[1];


                const img = new Image();
                img.src = `data:image/jpeg;base64,${originalImageBase64}`;

                img.onload = () => {
                    let width = img.width;
                    let height = img.height;


                    if (width > maxWidth) {
                        height *= maxWidth / width;
                        width = maxWidth;
                    }

                    if (height > maxHeight) {
                        width *= maxHeight / height;
                        height = maxHeight;
                    }


                    const canvas = document.createElement("canvas");
                    canvas.width = width;
                    canvas.height = height;
                    const ctx = canvas.getContext("2d");


                    ctx.drawImage(img, 0, 0, width, height);


                    canvas.toBlob(
                        (blob) => {
                            const compressedImage = new File([blob], file.name, {
                                type: "image/jpeg",
                                lastModified: Date.now(),
                            });

                            const readerCompressed = new FileReader();
                            readerCompressed.readAsDataURL(compressedImage);
                            readerCompressed.onload = () => {
                                const compressedImageBase64 =
                                    readerCompressed.result.split(",")[1];
                                resolve(compressedImageBase64);
                            };
                        },
                        "image/jpeg",
                        quality
                    );
                };
            };
            reader.onerror = (error) => {
                reject(error);
            };
        });
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProduct({ ...product, [name]: value });
    };

    const handleFileChange = (e) => {
        setSelectedFile(e.target.files[0])
    };

    const handleUpload = async () => {
        if (selectedFile) {
            try {
                const imageBase64 = await readFileAsBase64AndCompress(selectedFile);
                setProduct({ ...product, imageBase64 });
            } catch (error) {
                console.error("Error compressing image :", error);
            }

        }
        else {
            console.error("No file added!")
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const { token, role } = authState;
            const response = await fetch(`${API_BASE_URL}/create/product`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${token}`,
                    'Role': role
                },
                body: JSON.stringify(product)
            });
            if (response.ok) {
                const data = await response.json();
                console.log("Product created :", data);
                navigate("/admin/products");
            } else {
                console.error("Error creating product:", response.status, response.statusText);
            }

        } catch (error) {
            console.error("Error creating product:", error)
        };
    }

    return (
        <div>
            <h1>Add Product</h1>
            <form onSubmit={handleSubmit}>
                <label>Product Name:</label>
                <input
                    type="text"
                    name="productName"
                    value={product.productName}
                    onChange={handleInputChange}
                    required
                />
                <label>Description:</label>
                <textarea
                    name="description"
                    value={product.description}
                    onChange={handleInputChange}
                    required
                />
                <label>Price:</label>
                <input
                    type="text"
                    name="price"
                    value={product.price}
                    onChange={handleInputChange}
                    required
                />
                <label>Stock:</label>
                <input
                    type="text"
                    name="stock"
                    value={product.stock}
                    onChange={handleInputChange}
                    required
                />
                <label>Discount:</label>
                <input
                    type="text"
                    name="discount"
                    value={product.discount}
                    onChange={handleInputChange}
                    required
                />
                <label>Category:</label>
                <select
                    name="category"
                    value={product.category}
                    onChange={handleInputChange}
                    required
                >
                    <option value="">Select Category</option>
                    <option value="1">Dog</option>
                    <option value="2">Cat</option>
                </select>
                <label>SubCategory:</label>
                <select
                    name="subCategory"
                    value={product.subCategory}
                    onChange={handleInputChange}
                    required
                >
                    <option value="">Select SubCategory</option>
                    <option value="1">Games</option>
                    <option value="2">Accessories</option>
                    <option value="3">WetFood</option>
                    <option value="4">DryFood</option>
                </select>
                <label>Image (Base64):</label>
                <input
                    type="file"
                    accept=".jpg,.jpeg,.png"
                    onChange={handleFileChange}
                    required
                />
                <button type="button" onClick={handleUpload}>Upload Image</button>
                <button type="submit">Add Product</button>
            </form>
        </div>
    )
}

export default AddProduct