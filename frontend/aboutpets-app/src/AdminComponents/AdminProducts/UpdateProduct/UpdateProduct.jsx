import React from 'react'
import API_BASE_URL from '../../../config'
import { useEffect, useState, useContext } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { AuthContext } from '../../../AuthContext/AuthContext'
import './UpdateProduct.css'



const UpdateProduct = () => {
    const { authState } = useContext(AuthContext);
    const navigate = useNavigate();
    const { productId } = useParams();

    const [product, setProduct] = useState({
        productName: '',
        description: '',
        price: '',
        stock: '',
        discount: '',
        category: '',
        subCategory: '',
        imageBase64: ''
    })
    const [selectedFile, setSelectedFile] = useState(null);
    const [imageBase64, setImageBase64] = useState('');


    const readFileAsBase64AndCompress = (file, maxWidth = 800, maxHeight = 600, quality = 0.8) => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => {
                const originalImageBase64 = reader.result.split(",")[1];

                // Create an HTML image element to manipulate the image
                const img = new Image();
                img.src = `data:image/jpeg;base64,${originalImageBase64}`;

                img.onload = () => {
                    let width = img.width;
                    let height = img.height;

                    // Calculate the new dimensions while maintaining aspect ratio
                    if (width > maxWidth) {
                        height *= maxWidth / width;
                        width = maxWidth;
                    }

                    if (height > maxHeight) {
                        width *= maxHeight / height;
                        height = maxHeight;
                    }

                    // Create a canvas to draw the resized image
                    const canvas = document.createElement("canvas");
                    canvas.width = width;
                    canvas.height = height;
                    const ctx = canvas.getContext("2d");

                    // Draw the resized image on the canvas
                    ctx.drawImage(img, 0, 0, width, height);

                    // Convert the canvas content back to base64 with compression
                    canvas.toBlob(
                        (blob) => {
                            const compressedImage = new File([blob], file.name, {
                                type: "image/jpeg", // Adjust the MIME type if needed
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
                        "image/jpeg", // Adjust the MIME type if needed
                        quality // Adjust the quality (0-1) to control compression level
                    );
                };
            };
            reader.onerror = (error) => {
                reject(error);
            };
        });
    };
    const handleFileChange = (e) => {
        setSelectedFile(e.target.files[0]);
    };

    useEffect(() => {
        const fetchProduct = async () => {
            const { token, role } = authState;
            try {
                const response = await fetch(`${API_BASE_URL}/product/${productId}`, {
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    }
                });
                const data = await response.json();
                setProduct(data);
            } catch (error) {
                console.error("Error fetching product:", error);
            }
        };

        if (authState.token) {
            fetchProduct();
        }
    }, [authState, productId]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProduct({ ...product, [name]: value });
    };
    const handleUpload = async () => {
        if (selectedFile) {
            try {
                const imageBase64 = await readFileAsBase64AndCompress(selectedFile);
                setImageBase64(imageBase64);
                console.log(imageBase64); // Ellenőrizd, hogy kapott-e értéket
            } catch (error) {
                console.error("Error compressing image:", error);
            }
        } else {
            console.error("No file selected");
        }
    };
    const handleSubmit = async (e) => {
        e.preventDefault();
        const { token, role } = authState;

        try {
            if (imageBase64) {
                const updatedProduct = {
                    ...product,
                    imageBase64: imageBase64,
                };

                const response = await fetch(`${API_BASE_URL}/product/update/${productId}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                        'Authorization': `Bearer ${token}`,
                        'Role': role
                    },
                    body: JSON.stringify(updatedProduct)
                });

                if (response.ok) {
                    const data = await response.json();
                    console.log("Product updated:", data);
                    navigate('/admin/products');
                } else {
                    console.error("Error updating product:", response.status, response.statusText);
                }
            } else {
                console.error("No image selected");
            }
        } catch (error) {
            console.error("Error updating product:", error);
        }
    };
    return (
        <div>
            <h1>Update Product</h1>
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
                />
                <button type="button" onClick={handleUpload}>Upload Image</button>
                <button type="submit">Update Product</button>
            </form>

        </div>
    );
}

export default UpdateProduct