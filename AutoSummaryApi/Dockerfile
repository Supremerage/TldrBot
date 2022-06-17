FROM python:3

WORKDIR /app

COPY . .

ENV LC_ALL=C.UTF-8
ENV LANG=C.UTF-8

RUN pip install -r requirements.txt

EXPOSE 8000
CMD ["uvicorn", "main:app", "--port", "8000", "--host", "0.0.0.0"]