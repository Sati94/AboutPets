
import './App.css';
import Nav from './Components/Nav/Nav';
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Shop from './Pages/Shop';
import ShopCategory from './Pages/ShopCategory';
import Profile from './Pages/Profile';
import Products from './Pages/Products';
import Cart from './Pages/Cart';
import LoginSingup from './Pages/LoginSingup';

function App() {
  return (
    <div >
      <BrowserRouter>
        <Nav />
        <Routes>
          <Route path='/' element={<Shop />} />
          <Route path='/dogs' element={<ShopCategory category="dogs" />} />
          <Route path='/cats' element={<ShopCategory category="cats" />} />
          <Route path='/profileId' element={<Profile />} />
          <Route path='products' element={<Products />} >
            <Route path=':productsId' element={<Products />} />
          </Route>
          <Route path='/cart' element={<Cart />} />
          <Route path='/login' element={<LoginSingup />} />


        </Routes>
      </BrowserRouter>
    </div>
  );
}

export default App;
