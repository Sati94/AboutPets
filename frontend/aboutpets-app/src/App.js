import Cookies from 'js-cookie';
import { useState, useContext } from 'react';
import './App.css';
import Nav from './Components/Nav/Nav';
import { BrowserRouter as Router, Routes, Route, Link, useNavigate } from 'react-router-dom'
import Shop from './Pages/Shop';
import ShopCategory from './Pages/ShopCategory';
import Product from './Pages/Product'
import Profile from './Pages/Profile';
import Cart from './Pages/Cart';
import Login from './Pages/Login';
import ProductsDisplay from './Components/ProductsDisplay/ProductsDisplay';
import SubCategoryPage from './Pages/SubCategoryPage';
import Register from './Pages/Register';
import { AuthProvider, AuthContext } from './AuthContext/AuthContext'




const App = () => {

  /*const userId = Cookies.get("userId");
  const userName = Cookies.get("userUserName");
  const userToken = Cookies.get("userToken");
  const userEmail = Cookies.get("userEmail");
  const userRole = Cookies.get("Role");
  const orderId = Cookies.get("orderId");
*/

  /*const [isLoggedIn, setIsLoggedIn] = useState(!userId ? false : true);*/

  /*
    const handleLogin = () => {
      setIsLoggedIn(true);
    }
      */

  /* const handleLogout = () => {
     setIsLoggedIn(false);
     Cookies.remove("userId");
     Cookies.remove("userUserName");
     Cookies.remove("userToken");
     Cookies.remove("userEmail");
     Cookies.remove("Role");
     Cookies.remove("orderId");
 
 
   }
     */
  return (
    <div >
      <AuthProvider>
        <Router>
          <Nav />
          <Routes>
            <Route path='/' element={<Shop />} />
            <Route path='/dog' element={<ShopCategory category={1} />} />
            <Route path='/cat' element={<ShopCategory category={2} />} />
            <Route path='/action' element={<ProductsDisplay onlyDiscounted={true} />} />
            <Route path='/products' element={<ProductsDisplay />} />
            <Route path='/login' element={<Login />} />
            <Route path='/register' element={<Register />} />
            <Route path='/products/:productId' element={<Product />} />
            <Route path='/category/:category/:subCategory' element={<SubCategoryPage />} />



            <Route path='/profile' element={<Profile />} />
            <Route path='/cart' element={<Cart />} />



          </Routes>
        </Router>
      </AuthProvider>
    </div>
  );
}

export default App;
