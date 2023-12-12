import ProductCard from "../ProductCards/ProductCards";
import React from "react";
const Dashboard = ({ allPRoducts, isLoggedIn }) => {
    return (
        <React.Fragment>
            <h2>All Products</h2>

            {/* --- DASHBOARD FEATURES ---*/}
            <div className="tiles">
                {allPRoducts.length > 0 ? (
                    allPRoducts.map((product) => (
                        <React.Fragment key={product.id}>
                            <ProductCard product={product} isLoggedIn={isLoggedIn} />
                        </React.Fragment>
                    ))
                ) : (
                    <>No Product added yet...</>
                )}
            </div>
        </React.Fragment>
    );
};

export default Dashboard;
