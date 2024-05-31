
import './App.css';
import Nav from './Components/Nav/Nav';
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Shop from './Pages/Shop';
import ShopCategory from './Pages/ShopCategory';
import Profile from './Pages/Profile';
import Products from './Pages/Products';
import Cart from './Pages/Cart';
import LoginSingup from './Pages/LoginSingup';
import ProductsDisplay from './Components/ProductsDisplay/ProductsDisplay';
import SubCategoryPage from './Pages/SubCategoryPage';




function App() {


  return (
    <div >
      <BrowserRouter>
        <Nav />
        <Routes>
          <Route path='/' element={<Shop />} />
          <Route path='/dog' element={<ShopCategory category={1} />} />
          <Route path='/cat' element={<ShopCategory category={2} />} />
          <Route path='/action' element={<ProductsDisplay onlyDiscounted={true} />} />
          <Route path='/profileId' element={<Profile />} />
          <Route path='products' element={<ProductsDisplay />} >
            <Route path=':productsId' element={<Products />} />
          </Route>
          <Route path='/cart' element={<Cart />} />
          <Route path='/login' element={<LoginSingup />} />
          <Route path='/category/:category/:subCategory' element={<SubCategoryPage />} />


        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
