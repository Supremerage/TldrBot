from fastapi import FastAPI
from pydantic import BaseModel
from transformers import pipeline, set_seed

class Item(BaseModel):
    text: str

app = FastAPI()
# Load model
generator = pipeline("text-generation", model='distilgpt2') 
set_seed(42)

class TextGenerationInput(BaseModel):
    text: str
    temperature: float
    min_length: int
    max_length: int
    do_sample: bool

@app.post("/textgeneration")
async def generate_text(item: TextGenerationInput):
    summary = generator(item.text, max_length=item.max_length, min_length=item.min_length, do_sample=item.do_sample, temperature=item.temperature, top_p=0.9)
    return summary
