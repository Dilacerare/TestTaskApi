import React from 'react';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import SendMessages from './components/SendMessages';
import MessagesLast10Min from './components/MessagesLast10Min.jsx';
import WebSocketMessages from './components/WebSocketMessages';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<SendMessages />} />
                <Route path="/page1" element={<SendMessages />} />
                <Route path="/page2" element={<MessagesLast10Min />} />
                <Route path="/page3" element={<WebSocketMessages />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;