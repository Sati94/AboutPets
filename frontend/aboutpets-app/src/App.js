
import './App.css';
import Nav from './Components/Nav/Nav';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Shop from './Pages/Shop';
import ShopCategory from './Pages/ShopCategory';
import Product from './Pages/Product'
import Profile from './Pages/Profile';
import Cart from './Pages/Cart';
import Login from './Pages/Login';
import ProductsDisplay from './Components/ProductsDisplay/ProductsDisplay';
import SubCategoryPage from './Pages/SubCategoryPage';
import Register from './Pages/Register';
import ManageProductsPage from './AdminPages/ManageProductsPage';
import AdminToDo from './AdminComponents/AdminToDo/AdminToDo';
import UpdateProductPage from './AdminPages/UpdateProductPage';
import AddProductPage from './AdminPages/AddProductPage';
import ManageUseresPages from './AdminPages/ManageUseresPages';
import ManageOrdersPages from './AdminPages/ManageOrdersPages';
import UpdateOrdersPage from './AdminPages/UpdateOrdersPage';
import { AuthProvider } from './AuthContext/AuthContext';
import UpdateUserProfilePage from './AdminPages/UpdateUserProfilePage';
import Cookies from 'js-cookie';





const App = () => {

  /*const userId = Cookies.get("userId");
  const userName = Cookies.get("userUserName");
  const userToken = Cookies.get("userToken");
  const userEmail = Cookies.get("userEmail");
  const userRole = Cookies.get("Role");
 
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
  const orderId = Cookies.get("orderId");
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

            <Route path='/admin' element={<AdminToDo />} />
            <Route path='/admin/products' element={<ManageProductsPage />} />
            <Route path='/admin/product/:productId' element={<UpdateProductPage />} />
            <Route path='/admin/addproduct' element={<AddProductPage />} />
            <Route path='/admin/users' element={<ManageUseresPages />} />
            <Route path='/admin/users/:userId' element={<UpdateUserProfilePage />} />
            <Route path='admin/orders' element={<ManageOrdersPages />} />
            <Route path='admin/orders/:orderId' element={<UpdateOrdersPage />} />


          </Routes>
        </Router>
      </AuthProvider>
    </div>
  );
}

export default App;
