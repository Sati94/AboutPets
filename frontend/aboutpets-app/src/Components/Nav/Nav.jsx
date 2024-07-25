import React, { useState, useEffect, useContext } from 'react'
import './Nav.css'
import logo from '../Assets/logo.png'
import cart_icon from '../Assets/cart_icon.png'
import { Link } from 'react-router-dom'
import API_BASE_URL from '../../config'
import { AuthContext } from '../../AuthContext/AuthContext'
import { useNavigate } from 'react-router-dom'




const Nav = () => {

  const { authState, logout, setAuthState } = useContext(AuthContext);
  const [menu, setMenu] = useState("shop");
  const [orderItems, setOrderItems] = useState([]);
  const [length, setLength] = useState(0);
  const [isMenuOpen, setIsMenuOpen] = useState(false);
  const navigate = useNavigate();


  useEffect(() => {

    const fetchOrderItems = async () => {
      try {
        const { token, role, orderId } = authState;


        if (orderId) {
          const url = new URL(`${API_BASE_URL}/order/orderItems/${orderId}`);
          const response = await fetch(url, {
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${token}`,
              'Role': role
            },
          });
          if (!response.ok) {
            throw new Error('Failed to fetch the data!');
          }
          const data = await response.json();
          setOrderItems(data);
          setLength(data.length);


        }
      } catch (error) {
        console.error("Error fetching orderItems:", error);

        setAuthState(prevState => ({
          ...prevState,
          orderId: null
        }));

        navigate("/");
      }
    };

    if (authState.token && authState.orderId) {
      fetchOrderItems();
    } else {

      setLength(0);
    }
  }, [authState.token, authState.orderId, orderItems]);


  const handleMenuClick = (menuItem) => {
    setMenu(menuItem); // Menüpont állapotának frissítése
    // Navigálás az adott oldalra
    switch (menuItem) {
      case 'shop':
        navigate('/');
        break;
      case 'dog':
        navigate('/dog');
        break;
      case 'cat':
        navigate('/cat');
        break;
      case 'action':
        navigate('/action');
        break;
      case 'profile':
        navigate('/profile');
        break;
      case 'order':
        navigate('/my-orders')
        break;
      case 'adminDashboard':
        navigate('/admin');
        break;
      case 'manageUsers':
        navigate('/admin/users');
        break;
      case 'manageOrders':
        navigate('/admin/orders');
        break;
      case 'manageProducts':
        navigate('/admin/products');
        break;
      default:
        break;
    }
  };
  const handleLogout = () => {
    logout();
    navigate('/');
  }


  const isAuthenticated = !!authState.token;

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  }

  const renderUserMenu = () => (
    <div className='nav'>
      <div className='nav-logo'>
        <img src={logo} alt='' />
        <p>About Pets</p>
      </div>
      <div className={`nav-menu ${isMenuOpen ? 'open' : ''}`}>
        <ul className="nav-menu">
          <li className={menu === 'shop' ? 'active' : ''} onClick={() => handleMenuClick('shop')}>
            Shop
            {menu === 'shop' && <hr />}
          </li>
          <li className={menu === 'dog' ? 'active' : ''} onClick={() => handleMenuClick('dog')}>
            Dog
            {menu === 'dog' && <hr />}
          </li>
          <li className={menu === 'cat' ? 'active' : ''} onClick={() => handleMenuClick('cat')}>
            Cat
            {menu === 'cat' && <hr />}
          </li>
          <li className={menu === 'action' ? 'active' : ''} onClick={() => handleMenuClick('action')}>
            Discounts
            {menu === 'action' && <hr />}
          </li>
          <li className={menu === 'profile' ? 'active' : ''} onClick={() => handleMenuClick('profile')}>
            My Profile
            {menu === 'profile' && <hr />}
          </li>
          <li className={menu === 'order' ? 'active' : ''} onClick={() => handleMenuClick('order')}>
            My Orders
            {menu === 'order' && <hr />}
          </li>
        </ul>
      </div>
      {isAuthenticated ? (
        <div className='nav-login-cart'>
          <button className="logout-button" onClick={handleLogout}>Logout</button>
          <Link to='/cart'><img src={cart_icon} alt='' /></Link>
          <div className='nav-cart-count'>{length}</div>
        </div>
      ) : (
        <div className='nav-login-cart'>
          <Link to='/login'><button className="login-button">Login</button></Link>
        </div>
      )}
      <div className='nav-toggle' onClick={toggleMenu}>
        <div className='bar'></div>
        <div className='bar'></div>
        <div className='bar'></div>
      </div>
    </div>
  );

  const renderAdminMenu = () => (
    <div className='nav'>
      <div className='nav-logo'>
        <img src={logo} alt='Logo' />
        <p>About Pets</p>
      </div>
      <div className={`nav-menu ${isMenuOpen ? 'open' : ''}`}>

        <li className={menu === 'adminDashboard' ? 'active' : ''} onClick={() => handleMenuClick('adminDashboard')}>
          AdminToDo
          {menu === 'adminDashboard' && <hr />}
        </li>
        <li className={menu === 'manageUsers' ? 'active' : ''} onClick={() => handleMenuClick('manageUsers')}>
          Manage Users
          {menu === 'manageUsers' && <hr />}
        </li>
        <li className={menu === 'manageOrders' ? 'active' : ''} onClick={() => handleMenuClick('manageOrders')}>
          Manage Orders
          {menu === 'manageOrders' && <hr />}
        </li>
        <li className={menu === 'manageProducts' ? 'active' : ''} onClick={() => handleMenuClick('manageProducts')}>
          Manage Products
          {menu === 'manageProducts' && <hr />}
        </li>

      </div>
      {isAuthenticated && (
        <div className='nav-login-cart'>
          <button className="logout-button" onClick={handleLogout}>Logout</button>
        </div>
      )}
      <div className='nav-toggle' onClick={toggleMenu}>
        <div className='bar'></div>
        <div className='bar'></div>
        <div className='bar'></div>
      </div>
    </div>
  );

  if (authState.role === 'Admin') {
    return renderAdminMenu();
  }
  return renderUserMenu();
}

export default Nav