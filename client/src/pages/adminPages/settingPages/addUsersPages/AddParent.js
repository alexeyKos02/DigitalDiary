import React, {useEffect, useState} from 'react';
import AddChild from "../../../../components/addUserComponents/AddChild";
import {getAllStudents, setParent} from "../../../../http/ItemAPI";
import Button from "react-bootstrap/Button";
import {error, success} from "../../../../components/Notifications";
import Form from "react-bootstrap/Form";
const AddParent = ({firstName,lastName,email}) => {
    const [allStudents, setAllStudents] = useState([])
    const [childArray,setChildArray] = useState([])
    useEffect(()=>{
        getAllStudents().then(
            data=>{
                setAllStudents(data.data)
            }
        )
    },[])
    function addChild(arrayOfId){
        setChildArray(arrayOfId)
    }
    function click(){
        if (firstName ==="" || lastName === "" || email ==="" || childArray.length === 0){
            error("Заполните все поля.")
            return
        }
        setParent(firstName,lastName,email,childArray).then(_=>{
            success("Родитель добавлен.")
        }).catch(_ =>{
            error("Не удалось добавить родителя.")
        })
    }
    return (
        <div>
            <Form.Group className="mb-3">
                <Form.Label>Выберите детей</Form.Label>
                <AddChild array={allStudents} change={addChild}/>
            </Form.Group>
            <Button onClick={click}>Добавить родителя</Button>
        </div>
    );
};

export default AddParent;