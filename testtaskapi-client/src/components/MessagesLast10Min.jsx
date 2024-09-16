import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

function MessagesLast10Min() {
    const [messages, setMessages] = useState([]);

    useEffect(() => {
        fetchMessages();
    }, []);

    async function fetchMessages() {
        try {
            console.log('Запрос к API: http://localhost:5000/api/Message/all');
            const response = await fetch('http://localhost:5000/api/Message/all');
            if (response.ok) {
                const messages = await response.json();


                setMessages(messages);
                console.log('Получено сообщений:', messages.length);
            } else {
                console.error('Ошибка получения сообщений:', response.status);
            }
        } catch (error) {
            console.error('Ошибка получения сообщений:', error);
        }
    }

    return (
        <div className="container mt-4">
            <h1 className="mb-4">Сообщения за последние 10 минут</h1>
            <div id="messages">
                {messages.length === 0 ? (
                    <p>Нет сообщений за последние 10 минут.</p>
                ) : (
                    messages.map((message, index) => (
                        <div key={index}>
                            <strong>Время:</strong> {new Date(message.timestamp).toLocaleString()} <br />
                            <strong>Содержание:</strong> {message.content} <br />
                            <strong>Порядковый номер:</strong> {message.sequenceNumber}
                        </div>
                    ))
                )}
            </div>
            <div>
                <Link to="/page1">Отправить сообщение</Link>
                <br />
                <Link to="/page3">Сообщения по веб-сокету</Link>
            </div>
        </div>
    );
}

export default MessagesLast10Min;