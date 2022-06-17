from fastapi import FastAPI
from pydantic import BaseModel
from transformers import pipeline

class Item(BaseModel):
    text: str

app = FastAPI()
summarizer = pipeline("summarization", model="facebook/bart-large-cnn")

@app.post("/summary")
async def generate_auto_summary(item: Item, maxLength: int = 130, minLength: int = 30):
    summary = summarizer(item.text, max_length=maxLength, min_length=minLength, do_sample=False)
    return summary
