import React, { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { Link } from 'react-router-dom';

const WebSocketMessages = () => {
    const [messages, setMessages] = useState([]);

    useEffect(() => {
        const connection = new signalR.HubConnectionBuilder()
            .configureLogging(signalR.LogLevel.Debug)
            .withUrl("http://localhost:5000/messageHub", {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets
            })
            .build();

        connection.on("ReceiveMessage", (message) => {
            console.log("Получено сообщение:", message);
            setMessages(prevMessages => [message, ...prevMessages]); 
        });

        connection.start().catch(err => console.error(err.toString()));
        connection.start()
            .then(() => console.log('Соединение с WebSocket установлено'))
            .catch(err => { console.error('Ошибка подключения к WebSocket:', err);});

        return () => {
            connection.stop().catch(err => console.error(err.toString()));
        };
    }, []);

    return (
        <div className="container mt-4">
            <h1 className="mb-4">Сообщения по веб-сокету</h1>
            <div id="messages">
                {messages.map((message, index) => (
                    <div key={index} className="message bg-light border p-3 mb-2 rounded">
                        <strong>Содержание:</strong> {message.content} <br />
                        <strong>Время:</strong> {new Date(message.timestamp).toLocaleString()} <br />
                        <strong>Порядковый номер:</strong> {message.sequenceNumber} <br />
                    </div>
                ))}
            </div>
            <div>
                <Link to="/page1">Отправить сообщение</Link>
                <br />
                <Link to="/page2">Сообщения за последние 10 минут</Link>
            </div>
        </div>
    );
};


export default WebSocketMessages;