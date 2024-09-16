import React, { useState } from 'react';
import { Link } from 'react-router-dom';

function SendMessages() {
    const [message, setMessage] = useState('');
    const [response, setResponse] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (message.length > 128) {
            setResponse('Сообщение не может превышать 128 символов.');
            console.log('Ошибка отправки: Сообщение слишком длинное');
            return;
        }

        try {
            console.log('Отправка сообщения: ', message);

            const response = await fetch('http://localhost:5000/api/Message', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ Content: message }),
            });

            if (response.ok) {
                setResponse('Сообщение отправлено!');
                console.log('Сообщение отправлено успешно');
            } else {
                setResponse('Ошибка при отправки сообщения.');
                console.error('Ошибка отправки сообщения:', response.status);
            }
        } catch (error) {
            setResponse('Ошибка: ' + error.message);
            console.error('Ошибка при отправке сообщения:', error.message);
        }
    };

    return (
        <div className="container mt-4">
            <h1>Отправить сообщение</h1>
            <form id="messageForm" onSubmit={handleSubmit} className="mb-4">
                <div className="form-group">
                    <input
                        type="text"
                        id="messageContent"
                        className="form-control"
                        placeholder="Enter your message"
                        required
                        value={message}
                        onChange={(e) => setMessage(e.target.value)}
                    />
                </div>
                <button type="submit" className="btn btn-primary">
                    Send
                </button>
            </form>
            <div>
                <Link to="/page2">Сообщения за последние 10 минут</Link>
                <br />
                <Link to="/page3">Сообщения по веб-сокету</Link>
            </div>

            <div id="response" className={`alert ${response ? 'alert-info' : ''}`} role="alert">
                {response}
            </div>
        </div>
    );
}

export default SendMessages;
