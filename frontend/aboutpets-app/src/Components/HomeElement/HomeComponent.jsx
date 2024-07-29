import React from 'react';
import { Link } from 'react-router-dom';
import './HomeComponent.css';
import Footer from '../FooterItem/Footer';

const HomeComponent = () => {
    return (
        <div className='home-container'>
            <header className='home-header'>
                <h1>Welcome to Our Store</h1>
                <p>Experience the best shopping with exclusive offers.</p>
            </header>
            <main className='home-content'>
                <section className='info-section'>
                    <h2>Why Shop With Us?</h2>
                    <p>Only registered users can make purchases. Register now to unlock great deals and exclusive offers!</p>
                    <p>If you spend at least $100 on a purchase, you will receive a 10% discount coupon for your next purchase.</p>
                    <Link to='/register' className='register-link'>Register Now</Link>
                </section>
            </main>
            <footer className='home-footer'>
                <p>&copy; 2024 Our Store. All rights reserved.</p>
                <p>Contact us: info@ourstore.com</p>
            </footer>
            <Footer />
        </div>
    );
};

export default HomeComponent;