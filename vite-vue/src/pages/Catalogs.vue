<template>
    <div class="container-fluid bg-dark">
            <div class="card-header text-warning rounded hdr">
                Product Catalogs
            </div>        
            <div v-if="totpage ===0" class=" text-center text-danger">Products is empty....</div>
            <div v-else-if="totpage >1" class="text-danger">{{message}}</div>

            <div class="container-fluid">

            <div class="card-group">
                 <div v-for="prod in prods" :key="prod['id']">
                        <div class="card mx-2 mt-3 ">
                            <img v-bind:src="`http://localhost:5094/products/${prod['productpicture']}`" class="card-img-top" alt=""/>
                            <div class="card-body">
                                <h5 class="card-title">Description</h5>
                                <p class="card-text">{{ prod['descriptions'] }}</p>
                            </div>
                            <div class="card-footer">
                                <p class="card-text text-danger price-size"><span class="text-dark">PRICE :</span>&nbsp;<strong>&#8369;{{ toDecimal(prod['sellprice']) }}</strong></p>
                            </div>  
                        </div>
                    
                </div>                    
            </div>    
            </div>

            @if (totpage > 1) {
            <nav aria-label="Page navigation example">
              <ul class="pagination mt-2 mb-5">
                <li class="page-item"><a @click="lastPage($event)" class="page-link" href="#">Last</a></li>
                <li class="page-item"><a @click="prevPage($event)" class="page-link" href="#">Previous</a></li>
                <li class="page-item"><a @click="nextPage($event)" class="page-link" href="#">Next</a></li>
                <li class="page-item"><a @click="firstPage($event)" class="page-link" href="#">First</a></li>
                <li class="page-item page-link text-danger">Page&nbsp;{{page}} of&nbsp;{{totpage}}</li>
              </ul>
            </nav>
            }

    </div>
</template>

<style scoped>
nav {
    margin-left: 20px !important;
}
.card {
    width: 30vw !important;
}

.card-img-top {
    width: 30vw !important;
}
.card-body {
    height: 15vh !important;
}
.card-text {
    margin-left : -3px !important;
}
.card-title {
    margin-left: -2px !important;
}
.product-size {
    width: 240px!important;
    height: 340px!important;
}
.price-size {
    width: 215px;
}
.hdr {
    font-size: 24px;
    font-weight: bold;
    text-align: center;
}
</style>
<script setup lang="ts">
import { ref } from 'vue';
import axios from 'axios';

const api = axios.create({
    baseURL: "http://localhost:5094",
    headers: {'Accept': 'application/json',
              'Content-Type': 'application/json'}
})

const message = ref<string>('')
const page = ref<number>(1);
const totpage = ref<number>(0);
const totrecs = ref<number>(0);
const prods = ref<any[]>([]);

const getProducts = async (pg: number) => {
            const productsPayload = {
                query: `
                query ProductsList($input: PageInput!) {
                    listProducts(input: $input) {
                    product {
                      id
                      descriptions
                      qty
                      unit
                      costprice
                      sellprice
                      productpicture
                    }
                    page
                    totalpage
                    totalrecords
                  }
                }
                `,
                variables: {
                    input: { // Must match the $input variable name in your mutation
                        page: pg,
                    }
                }
            };

            try {
                const res = await api.post('/graphql', productsPayload); 
                
                if (res.data.errors) {
                    message.value = res.data.errors[0].message;
                    setTimeout(() => {
                      message.value = '';
                      
                    }, 3000);
                    return;
                } else {
                    const result = res.data.data?.listProducts; 
                    prods.value = result.product;  
                    page.value = result.page;
                    totpage.value = result.totalpage;
                    totrecs.value =result.totalrecords;
                }
            } catch (error: any) {
                message.value = error.response?.data?.errors?.[0]?.message || error.message || "An error occurred";
            } finally {
                setTimeout(() => { message.value = ''; }, 3000);
            }
}

getProducts(page.value);


const nextPage = (event: any) => {
    event.preventDefault();    
    if (page.value == totpage.value) {
        return;
    }
    page.value++;
    getProducts(page.value);
    return;
}

const prevPage = (event: any) => {
    event.preventDefault();    
    if (page.value == 1) {
    return;
    }
    page.value--;
    getProducts(page.value);
    return;    
}

const firstPage =  (event: any) => {
    event.preventDefault();    
    page.value = 1;
    getProducts(page.value);
    return;    
}

const lastPage = (event: any) => {
    event.preventDefault();    
    page.value = totpage.value;
    getProducts(page.value);
    return;    
}

const toDecimal = (number: number) => {
  const formatter = new Intl.NumberFormat('en-US', {
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  });
  return formatter.format(number);
};


</script>