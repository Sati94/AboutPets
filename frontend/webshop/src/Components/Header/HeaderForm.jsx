import { useParams, useLocation } from "react-router-dom";
import { Outlet, Link } from "react-router-dom";
import React, { useState } from "react";

const Header = ({ isLoggedIn, userName, onLogout }) => {
    const [isMobileNavVisible, setIsMobileNavVisible] = useState(false);
    const toggleMobileNav = () => {
        setIsMobileNavVisible(!isMobileNavVisible);
    };
    return (
        <React.Fragment>
            <header className="app-header">
                <div className="app-header-logo">
                    <div className="logo">
                        <span className="logo-icon">
                            <img src="https://assets.codepen.io/285131/almeria-logo.svg" />
                        </span>
                        <h1 className="logo-title">
                            <span>About</span>
                            <span>Pets</span>
                        </h1>
                    </div>
                </div>

                <div className="app-header-navigation">
                    <div className="tabs">
                        <Link to="/product/avaiable">Products</Link>

                        <Link to="/">Trending Products</Link>

                        <Link to="/marketplace/you/account">Account</Link>

                        <Link to="/">Business</Link>
                        <Link to="/">System</Link>
                    </div>
                </div>

                <div className="app-header-actions">
                    {isLoggedIn ? (
                        <>
                            <li>Hello, {userName}!</li>
                            <li>
                                <button className="logout-btn" onClick={onLogout}>
                                    Logout
                                </button>
                            </li>
                        </>
                    ) : (
                        <>
                            <Link to="/login">Login</Link>

                            <Link to="/register">Register</Link>
                        </>
                    )}
                </div>

                <>
                    <button className="icon-button large" onClick={toggleMobileNav}>
                        <svg
                            xmlns="http://www.w3.org/2000/svg"
                            width="16"
                            height="16"
                            fill="currentColor"
                            className="bi bi-list"
                            viewBox="0 0 16 16"
                        >
                            <path
                                fillRule="evenodd"
                                d="M2.5 12a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5zm0-4a.5.5 0 0 1 .5-.5h10a.5.5 0 0 1 0 1H3a.5.5 0 0 1-.5-.5z"
                            />
                        </svg>
                    </button>
                    {isMobileNavVisible && (
                        <div className="app-header-mobile">
                            <div className="">
                                <div className="tabs">
                                    <Link to="/product/create" onClick={toggleMobileNav}>
                                        Add Products
                                    </Link>

                                    <Link to="/marketplace" onClick={toggleMobileNav}>
                                        Marketplace
                                    </Link>
                                    <Link to="/" onClick={toggleMobileNav}>
                                        Trending Products
                                    </Link>
                                    <Link to="/marketplace/you/account" onClick={toggleMobileNav}>
                                        Account
                                    </Link>
                                    {isLoggedIn ? (
                                        <>
                                            <li>Hello, {userName}!</li>
                                            <li>
                                                <button className="logout-btn" onClick={onLogout}>
                                                    Logout
                                                </button>
                                            </li>
                                        </>
                                    ) : (
                                        <>
                                            <Link to="/">Login</Link>

                                            <Link to="/register">Register</Link>
                                        </>
                                    )}
                                </div>
                            </div>
                        </div>
                    )}
                </>
            </header>
            <Outlet />
        </React.Fragment>
    );
};

export default Header;