from flask import Flask, jsonify, render_template_string
import json
import random

app = Flask(__name__)

# Load quotes from JSON file
with open('quotes.json', 'r') as f:
    quotes = json.load(f)

HTML = """
<!DOCTYPE html>
<html>
<head>
    <title>Quote of the Day</title>
    <style>
        body { font-family: Arial, sans-serif; background: #f7f7fa; display: flex; flex-direction: column; align-items: center; justify-content: center; height: 100vh; }
        .container { background: #fff; padding: 40px 30px; border-radius: 12px; box-shadow: 0 2px 16px rgba(0,0,0,0.08); text-align: center; }
        h1 { color: #e75480; margin-bottom: 20px; }
        .quote { font-size: 1.3em; margin-bottom: 10px; color: #333; }
        .author { color: #888; font-style: italic; }
        button { margin-top: 20px; padding: 8px 18px; border: none; background: #e75480; color: #fff; border-radius: 6px; cursor: pointer; font-size: 1em; }
        button:hover { background: #d1436a; }
    </style>
</head>
<body>
    <div class="container">
        <h1>Quote of the Day</h1>
        <div id="quote-block">
            <div class="quote" id="quote"></div>
            <div class="author" id="author"></div>
        </div>
        <button onclick="getQuote()">New Quote</button>
    </div>
    <script>
        async function getQuote() {
            const res = await fetch('/quote');
            const data = await res.json();
            document.getElementById('quote').textContent = data.quote;
            document.getElementById('author').textContent = '- ' + data.author;
        }
        // Load a quote on page load
        getQuote();
    </script>
</body>
</html>
"""

@app.route('/')
def home():
    return render_template_string(HTML)

@app.route('/quote')
def quote():
    q = random.choice(quotes)
    return jsonify(q)

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
