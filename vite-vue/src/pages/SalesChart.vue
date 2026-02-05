<template>
    <main>
    <div class="container">
            <h1 class="text-center text-white">Annual Sales Chart</h1>
    </div>
    </main><
</template>

<script lang="ts">
import axios from 'axios';
import { ref } from 'vue';

const api = axios.create({
    baseURL: "http://localhost:5094",
    headers: {'Accept': 'application/json',
                'Content-Type': 'application/json'}
});

const sales = ref<any[]>([]);
const message = ref<string>('');

const getSales = async () => {
    const salesData = {
        query: `
        query ChartData {
           salesdata {
                amount
                monthdate
            }  
        }
        `
    };

    try {
        const res = await api.post('/graphql', salesData); 
        
        if (res.data.errors) {
            alert("error 1");
            message.value = res.data.errors[0].message;
            setTimeout(() => {
                message.value = '';
                
            }, 3000);
            return;
        } else {
            const result = res.data.data.salesdata; 
            sales.value = result.sales;
            return;
        }
    } catch (error: any) {
        message.value = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
    } finally {
        setTimeout(() => { message.value = ''; }, 3000);
    }
}

getSales();



</script>