import { Outlet, Link } from "react-router-dom";

const Nav = () => {
    return (
        <div className="app-body-navigation">
            <nav className="navigation">
                <li className="ph-globe">
                    <Link to="/product/create">Add Products</Link>
                </li>

                <li className="ph-browsers">
                    <Link to="/">Home</Link>
                </li>

                <li className="ph-check-square">
                    <Link to="/">My Orders</Link>
                </li>

                <li className="ph-swap">
                    <Link to="/">Explore</Link>
                </li>

                <li className="ph-clipboard-text">
                    <Link to="/">My Account</Link>
                </li>
            </nav>
            <footer className="footer">
                <h1>
                    About Pets<small>©</small>
                </h1>
                <div>
                    About Pets ©<br />
                    All Rights Reserved 2023
                </div>
            </footer>
        </div>
    );
};
export default Nav;